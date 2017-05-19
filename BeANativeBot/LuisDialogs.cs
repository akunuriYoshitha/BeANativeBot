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
        public string msg1;
        public void LuisDialogs_initialize(string msg2)
        {
            msg1 = msg2;
            return;
        }
        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            PromptDialog.Confirm(context, ConfirmAboutPrompt, $"" + msg1 + Environment.NewLine + 
                Environment.NewLine + "Do you want to know something about me?");

        }

        public async Task ConfirmAboutPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                await context.PostAsync($"I help you find the most attraction places around you." + Environment.NewLine + Environment.NewLine +
                    "I love to learn your interests and suggest places accordingly" + Environment.NewLine + Environment.NewLine +
                    "You can ask me to : " + Environment.NewLine + Environment.NewLine + 
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
            await context.PostAsync($"I help you find the most attraction places around you." + Environment.NewLine + Environment.NewLine + "I love to learn your interests and suggest places accordingly");
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
            await context.PostAsync($"Details are as follows....");
        }
        [LuisIntent("Response")]
        public async Task UserAcceptance(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hope you will enjoy atleast few of my suggestions...");
        }
        /*
        private async Task ConfirmAboutPrompt(IDialogContext context, IAwaitable<bool> result)
        {
            await context.PostAsync($"in confirm about prompt");
            if (await result)
            {
                await context.PostAsync($"I help you find the most attraction places around you." + Environment.NewLine + Environment.NewLine +
                    "I love to learn your interests and suggest places accordingly");
            }
            else
            {
                await context.PostAsync($"Hope you already heard about me" + Environment.NewLine + Environment.NewLine +
                    "If not, you can know about me any time when you are free");
            }
        }*/

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