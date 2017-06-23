using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;

namespace BeANativeBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = activity.CreateReply($"");
                Activity reply1 = activity.CreateReply($"");
                if (activity.Text.Contains("Place Id : "))
                {

                    var place_id = activity.Text.ToString().Substring(11, activity.Text.ToString().Length - 11);
                    int index = (int)Char.GetNumericValue(place_id[0]);
                    TextSearch ts = new TextSearch();

                    string refe = ts.getReference(index);
                    PlaceDetails pd = new PlaceDetails();
                    Attachment attachment = pd.makeCard(refe, place_id.Substring(1, place_id.Length - 1));
                    reply.Attachments = new List<Attachment>();
                    reply.Attachments.Add(attachment);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                    string review = pd.GetReviews(place_id.Substring(1, place_id.Length - 1));
                    reply1 = activity.CreateReply($"" + review);
                    await connector.Conversations.ReplyToActivityAsync(reply1);
                    var res = Request.CreateResponse(HttpStatusCode.OK);
                    return res;
                }

                await Conversation.SendAsync(activity, () => new LuisDialogs(activity.Text.ToString()));
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}