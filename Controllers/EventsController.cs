using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using thebookinglib.Models;
using thebookinglib.Repohelper;

namespace thebookingsapi.Controllers {

    [Route ("api/events")]
    [ApiController]
    public class EventsController : ControllerBase {
        DataHelper dataHelper;
        public EventsController () {
            dataHelper = new DataHelper ();
        }
        /// <summary>
        /// retrieves all the current events that have been added in the db or in the json flatfiles
        /// </summary>
        [HttpGet]
        [Route ("GetEvents")]
        public async Task<List<ClientEvent>> GetEvents () => await dataHelper.GetEvents ();
        /// <summary>
        /// Submit booking allows users to book a certain event that they would like to go to
        /// </summary>
        [HttpPost]
        [Route ("submitBooking")]
        public int ClientBooking ([FromBody] ClientBooking bookingJsonstring) {
            try {

                dataHelper.ClientBooking (bookingJsonstring);
                return 1;
            } catch (System.Exception ex) {
                throw new Exception (ex.InnerException.ToString ());
                return 0;

            }
        }
        /// <summary>
        /// subscribe to event adds users to the a specific events this and also include the communication method admin user or host will send the notifications 
        //if anything changes to a specific event
        /// </summary>
        [HttpPost]
        [Route ("subscribeToEvent")]
        public int SubscribeToEvent ([FromBody] Subscription subscriptionJsonstring) {
            try {

                dataHelper.SubscribeToEvent (subscriptionJsonstring);

                return 1;

            } catch (System.Exception ex) {
                throw new Exception (ex.InnerException.ToString ());
                return 0;
            }
        }

    }
}