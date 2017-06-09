using Be_A_Native;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BeANativeBot
{
    [LuisModel("ab168c9f-63a9-4b78-a542-abcf1d5e6704", "6b7d416d2dd8485fa78567299ada848a")]
    [Serializable]

    public class LuisDialogs : LuisDialog<object>
    {
        public string message;
        public LuisDialogs(string message)
        {
            this.message = message;
        }

        public static CardAction returnCardButton(string url)
        {
            CardAction CardButton = new CardAction()
            {
                Type = "openUrl",
                Title = "AiHelpWebsite.com",
                Value = url,
            };
            return CardButton;
        }
        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            PromptDialog.Confirm(context, ConfirmAboutPrompt, $"" + message + Environment.NewLine + 
                Environment.NewLine + "Do you want to know something about me?");

        }

        public async Task ConfirmAboutPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                await context.PostAsync($"I help you find the most attraction places around you." + Environment.NewLine + Environment.NewLine +
                    "I love to learn your interests and suggest places accordingly" + Environment.NewLine + Environment.NewLine +
                    "For example, You can ask me to : " + Environment.NewLine + Environment.NewLine + 
                    "   Show places around hyderabad" + Environment.NewLine + Environment.NewLine + 
                    "   Explain the significance of charminar" + Environment.NewLine + Environment.NewLine);
            }
            else
            {
                await context.PostAsync($"Hope you already heard about me" + Environment.NewLine + Environment.NewLine +
                    "If not, you can know about me any time when you are free");
            }
        }

        [LuisIntent("About")]
        public async Task About(IDialogContext context, LuisResult result)
        {
            
            await context.PostAsync($"I help you find the most attraction places around you." + Environment.NewLine + Environment.NewLine +
                    "I love to learn your interests and suggest places accordingly" + Environment.NewLine + Environment.NewLine +
                    "For example, You can ask me to : " + Environment.NewLine + Environment.NewLine +
                    "   Show places around hyderabad" + Environment.NewLine + Environment.NewLine +
                    "   Explain the significance of charminar" + Environment.NewLine + Environment.NewLine);
            context.Wait(MessageReceived);
            
            
        }

        [LuisIntent("AskPlaces")]
        public async Task AskPlaces(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Searching....");
            string placename = "";
            EntityRecommendation rec;
            if (result.TryFindEntity("place", out rec))
            {
                placename = rec.Entity;
                RestClient rs = new RestClient();
                string http_response = rs.MakeRequest(placename);
                await context.PostAsync($"" + http_response);
            }
            else
            {
                await context.PostAsync($"Could not find place....");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("AskDetails")]
        public async Task AskDetails(IDialogContext context, LuisResult result)
        {
            string placename = "";
            string placeId;
            string address = "Sorry!!! Not Available";
            EntityRecommendation rec;
            DetailsPlace dp = new DetailsPlace();
            if (result.TryFindEntity("place", out rec))
            {
                placename = rec.Entity;
                RestClient rs = new RestClient();
                rs.MakeRequest(placename);
                placeId = rs.retPlaceID(placename);
                
                address = dp.MakeRequest(placeId);
                //await context.PostAsync($"" + address);
            }
            else
            {
                await context.PostAsync($"Could not find place....");
            }


            Activity replyToConversation = (Activity)context.MakeMessage();
            replyToConversation.Recipient = replyToConversation.Recipient;
            replyToConversation.Type = "message";
            
            CardAction cardButtons = returnCardButton(dp.getRouteMap());
            HeroCard plCard = new HeroCard()
            {
                Title = "" + placename,
                Subtitle = "" + address + Environment.NewLine + Environment.NewLine 
                 + "Click to view the route map..." + Environment.NewLine + Environment.NewLine,
                Tap = cardButtons,
               
            };
            Attachment plAttachment = plCard.ToAttachment();
            // Attach the Attachment to the reply
            replyToConversation.Attachments.Add(plAttachment);
            // set the AttachmentLayout as 'list'
            replyToConversation.AttachmentLayout = "list";
            // Send the reply
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Response")]
        public async Task UserAcceptance(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hope you will enjoy atleast few of my suggestions...");
        }
        
        [LuisIntent("EndConversation")]
        public async Task EndConversation(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Bye...." + Environment.NewLine + Environment.NewLine +
                "Please visit again" + Environment.NewLine + Environment.NewLine + 
                "Hope you have a nice day..." + Environment.NewLine);
            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Could not get you... Please try again...");
            context.Wait(MessageReceived);
        }
    }
}