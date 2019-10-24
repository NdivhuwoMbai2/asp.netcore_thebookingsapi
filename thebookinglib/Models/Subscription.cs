using System.Collections.Generic;

namespace thebookinglib.Models {
    public class Subscription {
        public string eventname { get; set; }
        public CommunicationMethod communicationMethod { get; set; }
        public Users user = new Users ();
        public string message{get;set; }
    }

}