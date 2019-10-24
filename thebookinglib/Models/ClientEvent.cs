using System.Collections.Generic;

namespace thebookinglib.Models {
    public class ClientEvent {
        public string eventName { get; set; }
        public EventType eventtype { get; set; }
        public int eventId { get; set; }
        public List<Subscription> subscription { get; set; }

    }
}