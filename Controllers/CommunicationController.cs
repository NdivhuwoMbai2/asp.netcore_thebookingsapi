using Microsoft.AspNetCore.Mvc;
using thebookinglib.Repohelper;

namespace thebookingsapi.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class CommunicationController : ControllerBase {
        DataHelper dataHelper;
        public CommunicationController () {
            dataHelper = new DataHelper ();
        }
        /// <summary>
        /// Returns a group of Employees matching the given first and last names.
        /// </summary>
        /// <remarks>
        /// Here is a sample remarks placeholder.
        /// </remarks>
        /// <param name="firstName">The first name to search for</param>
        /// <param name="lastName">The last name to search for</param>
        /// <returns>A string status</returns>
        [HttpPost]
        [Route ("sendMessageToSubscribers")]
        public async void SendMessageToSubscribers (string textMessage, string eventId, string communicationMethod) {

            await dataHelper.SendMessageToSubscribers (
                textMessage,
                eventId,
                communicationMethod);
        }

    }
}