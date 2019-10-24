using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using thebookinglib.Models;

namespace thebookinglib.Repohelper {
    public class DataHelper {
        Users admin;
        //this is acting like a table from a database containing all the client events
        List<ClientEvent> events1 = new List<ClientEvent> ();
        //this is acting like a client booking pulled from the database which the repo can add more booking
        List<ClientBooking> clientBookings = new List<ClientBooking> ();

        public DataHelper () {
            //initiate admin user who the email is being sent from this could be returned from the database or the json file
            Users admin = new Users ();

        }

        private const int _clientcount = 15;
        private SmtpClient[] _smtpClients = new SmtpClient[_clientcount + 1];

        internal Task<List<ClientEvent>> GetEvents () {

            List<ClientEvent> events1 = new List<ClientEvent> ();

            IEnumerable<ClientEvent> events = from e in events1
            select e;
            return Task.FromResult (events.ToList ());
        }

        private CancellationTokenSource _cancelToken;

        public Task<int> SendMessageToSubscribers (string textMessage, string eventName, string communicationMethod) {

            Enum.TryParse (communicationMethod, out CommunicationMethod communicationType);

            try {

                //retrive data from the database or the json dabasource
                List<ClientEvent> clientSubscription = new List<ClientEvent> ();

                var subscriptioncategory = from c in clientSubscription where
                c.eventName == eventName
                select c;

                ParallelOptions po = new ParallelOptions ();
                //Create a cancellation token so you can cancel the task.
                _cancelToken = new CancellationTokenSource ();
                po.CancellationToken = _cancelToken.Token;

                //Manage the MaxDegreeOfParallelism instead of .NET Managing this. We dont need 500 threads spawning for this.
                po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2;
                try {
                    Parallel.ForEach (subscriptioncategory.SelectMany (e => e.subscription).Where (a => a.communicationMethod == communicationType), po, (Subscription subscription) => {
                        try {

                            switch (communicationType) {

                                case CommunicationMethod.EMAIL:

                                    MailMessage msg = new MailMessage (admin.userEmailAddress, subscription.user.userEmailAddress);
                                    msg.Subject = eventName;
                                    msg.Body = textMessage;
                                    msg.Priority = MailPriority.Normal;
                                    SendEmail (msg);
                                    break;
                                case CommunicationMethod.SMS:
                                    break;
                                case CommunicationMethod.WHATSAPP:
                                    break;
                                default:
                                    throw new Exception ($" An error occured while trying to send a message for this event {eventName} no communication method specified for user{subscription.user.personName}");

                            }

                        } catch (Exception ex) {
                            //Log error
                        }
                    });
                } catch (OperationCanceledException e) {
                    //User has cancelled this request.
                }
            } finally {
                disposeSMTPClients ();
            }

            return Task.FromResult (1);

        }

        internal void SubscribeToEvent (Subscription subscription) {
            ClientEvent eventTosubto = events1.Select (e => e).Where (admin => admin.eventName == subscription.eventname).FirstOrDefault ();
            eventTosubto.subscription.Add (subscription);
        }

        internal void ClientBooking (ClientBooking clientBooking) {
            clientBookings.Add(clientBooking);
        }

        public void CancelEmailRun () {
            _cancelToken.Cancel ();
        }

        private void SendEmail (MailMessage msg) {
            try {
                bool _gotlocked = false;
                while (!_gotlocked) {
                    //Keep looping through all smtp client connections until one becomes available
                    for (int i = 0; i <= _clientcount; i++) {
                        if (System.Threading.Monitor.TryEnter (_smtpClients[i])) {
                            try {
                                _smtpClients[i].Send (msg);
                            } finally {
                                System.Threading.Monitor.Exit (_smtpClients[i]);
                            }
                            _gotlocked = true;
                            break;
                        }
                    }
                    System.Threading.Thread.Sleep (2);
                }
            } finally {
                msg.Dispose ();
            }
        }
        private void setupSMTPClients () {
            for (int i = 0; i <= _clientcount; i++) {
                try {
                    SmtpClient _client = new SmtpClient ("127.0.0.1", 25);
                    //If your SMTP server requires authentication do the following below
                    _client.Credentials = new System.Net.NetworkCredential (
                        "adminusername",
                        "adminPassword",
                        "thedomain");
                    _smtpClients[i] = _client;
                } catch (Exception ex) {
                    throw new Exception ("An error occured while trying to send out an email", ex.InnerException);
                }
            }
        }
        private void disposeSMTPClients () {
            for (int i = 0; i <= _clientcount; i++) {
                try {
                    _smtpClients[i].Dispose ();
                } catch (Exception ex) {
                    throw new Exception ("An error occured while trying to dispose smtp", ex.InnerException);
                }
            }
        }
    }
}