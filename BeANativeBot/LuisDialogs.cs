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
        public const string DevotionalPlaces = "1. Devotional Places";
        public const string Education = "2. Education";
        public const string FoodCourts = "3. Food Courts";
        public const string Hospitals = "4. Hospitals";
        public const string Shopping = "5. Shopping";
        public const string Spa = "6. Spa";
        public const string TouristPlaces = "7. Tourist Places";

        public const string Temples = "1. Temples";
        public const string Churches = "2. Churches";
        public const string Mosques = "3. Mosques";

        public const string Schools = "1. Schools";
        public const string Inter = "2. Intermediate Colleges";
        public const string Engineering = "3. Engineering Colleges";
        public const string Medical = "4. Medical Colleges";
        public const string Polytechnic = "5. Polytechnic colleges";
        public const string Library = "6. Libraries";
        public const string BookStores = "7. Book Stores";

        public const string ShoppingMalls = "1. Shopping Malls";
        public const string JewelryStores = "2. Jewelry Stores";
        public const string MobileStores = "3. Mobile Stores";
        public const string CarLeasing = "4. Car Leasing";

        public const string BeautyParlour = "1. Beauty Parlour";
        public const string Salons = "2. Salons";
        public const string Gym = "3. Gym";

        public const string Museums = "1. Museums";
        public const string Zoos = "2. Zoos";
        public const string ArtGalleries = "3. Art Galleries";
        public const string Parks = "4. Parks";
        public const string Resorts = "5. Resorts";
        public const string FamousPlaces = "6. Famous Places";
        
        public static string placename = "";
        public static string category = "";
        private IEnumerable<string> options = new List<string> {DevotionalPlaces, Education, FoodCourts, Hospitals, Shopping, Spa, TouristPlaces};
        private IEnumerable<string> devinePlaces = new List<string> { Temples, Churches, Mosques };
        private IEnumerable<string> EducationCat = new List<string> { Schools, Inter, Engineering, Medical, Polytechnic, Library, BookStores };
        private IEnumerable<string> ShoppingPlaces = new List<string> { ShoppingMalls, JewelryStores, MobileStores, CarLeasing };
        private IEnumerable<string> SpaPlaces = new List<string> { BeautyParlour, Salons, Gym };
        private IEnumerable<string> TourismPlaces = new List<string> { Museums, Zoos, ArtGalleries, Parks, Resorts, FamousPlaces };

        public string message;
        public LuisDialogs(string message)
        {
            this.message = message;
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
                    "" + Environment.NewLine + Environment.NewLine +
                    "For example, You can ask me to : " + Environment.NewLine + Environment.NewLine + 
                    "   Show places around hyderabad" + Environment.NewLine + Environment.NewLine + 
                    "   Give the details of charminar" + Environment.NewLine + Environment.NewLine);
            }
            else
            {
                await context.PostAsync($"Hope you already heard about me" + Environment.NewLine + Environment.NewLine +
                    "If not, you can know about me any time when you are free");
            }
        }

        [LuisIntent("About")]
        public async Task About(IDialogContext context, IAwaitable<IMessageActivity> res, LuisResult result)
        {
            
            await context.PostAsync($"I help you find the most attraction places around you." + Environment.NewLine + Environment.NewLine +
                    "I love to learn your interests and suggest places accordingly" + Environment.NewLine + Environment.NewLine +
                    "For example, You can ask me to : " + Environment.NewLine + Environment.NewLine +
                    "   Show places around hyderabad" + Environment.NewLine + Environment.NewLine +
                    "   Give the details of charminar" + Environment.NewLine + Environment.NewLine);
            context.Wait(MessageReceived);
            
            
        }

        [LuisIntent("PlaceNames")]
        public async Task PlaceNames(IDialogContext context, LuisResult result)
        {
            EntityRecommendation rec;

            if (result.TryFindEntity("place", out rec))
            {
                placename = rec.Entity;
                await context.PostAsync($"place : " + placename);

            }
            SuggestPlaces(context, result);
            
            
        }
        [LuisIntent("AskPlaces")]
        public async Task AskPlaces(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Searching....");
            //context.Wait(MessageReceived);
            
            //await context.PostAsync($"Searching...." + result);
            
            string types = "";
            EntityRecommendation rec, rec_cat;
            
            if (result.TryFindEntity("place", out rec))
            {
                placename = rec.Entity;
                await context.PostAsync($"place : " + placename);
                
            }
            else
            {
                placename = "";
                await context.PostAsync($"Could not find place....");
                
            }
            if (result.TryFindEntity("category", out rec_cat))
            {
                category = rec_cat.Entity;
                await context.PostAsync($"category : " + category);
            }
            else
            {
                category = "";
                await context.PostAsync($"category not identified");
            }
            SuggestPlaces(context, result);
            
        }

        public async Task SuggestPlaces (IDialogContext context, LuisResult result)
        {
            if (placename != "" && category != "")
            {
                TextSearch ts = new TextSearch();
                var attachments = ts.makeRequest(placename, category);
                if (attachments.Count == 0)
                {
                    context.PostAsync("Sorry!!! No " + category + " available..." + Environment.NewLine + Environment.NewLine +
                        "You can try some of these...");
                    PromptCategories(context, result);
                    return;
                }

                Activity msg = (Activity)context.MakeMessage();
                msg.Recipient = msg.Recipient;
                msg.Type = "message";
                msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                msg.Attachments = attachments;

                await context.PostAsync(msg);
            }
            else if (placename != "" && category == "")
            {
                PromptCategories(context, result);
                return;
            }
            else
            {
                await context.PostAsync("Please enter your place");
            }
            context.Wait(MessageReceived);
        }

        public async Task PromptCategories(IDialogContext context, LuisResult result)
        {
            PromptDialog.Choice<string>(
                context,
                this.DisplaySelectedCard,
                this.options,
                "I can suggest places in seven categories. Select based on your preference....",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        
        public async Task DisplaySelectedCard(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            var type = "";
            await context.PostAsync("You have selected " + selectedCard);
            switch (selectedCard)
            {
                case DevotionalPlaces: DevotionalPrompt(context);
                    break;
                case Education: EducationPrompt(context);
                    break;
                case Shopping: ShoppingPrompt(context);
                    break;
                case Spa: SpaPrompt(context);
                    break;
                case TouristPlaces: TourismPrompt(context);
                    break;
                case FoodCourts:
                    TextSearch ts = new TextSearch();
                    var attachment = ts.makeRequest(placename, "restaurants");
                    Activity msg = (Activity)context.MakeMessage();
                    msg.Recipient = msg.Recipient;
                    msg.Type = "message";
                    msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    msg.Attachments = attachment;
                    await context.PostAsync(msg);
                    context.Wait(MessageReceived);
                    break;
                case Hospitals:
                    TextSearch tsh = new TextSearch();
                    var attachmenth = tsh.makeRequest(placename, "hospitals");
                    Activity msgh = (Activity)context.MakeMessage();
                    msgh.Recipient = msgh.Recipient;
                    msgh.Type = "message";
                    msgh.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    msgh.Attachments = attachmenth;
                    await context.PostAsync(msgh);
                    context.Wait(MessageReceived);
                    break;
            }
            //context.Wait(MessageReceived);
        }

        public async Task DevotionalPrompt(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.GetDevotionalPlaces,
                this.devinePlaces,
                "What kind of devine place do you want to visit???",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task GetDevotionalPlaces(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            var type = "";
            await context.PostAsync("You have selected " + selectedCard);
            switch (selectedCard)
            {
                case Temples: type = "temple";
                    break;
                case Churches: type = "church";
                    break;
                case Mosques: type = "mosque";
                    break;
            }
            TextSearch ts = new TextSearch();
            var attachment = ts.makeRequest(placename, type);
            Activity msg = (Activity)context.MakeMessage();
            msg.Recipient = msg.Recipient;
            msg.Type = "message";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = attachment;
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        public async Task EducationPrompt(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.GetEducationPlaces,
                this.EducationCat,
                "What kind of educational places do you want to visit???",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task GetEducationPlaces(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            var type = "";
            await context.PostAsync("You have selected " + selectedCard);
            switch (selectedCard)
            {
                case Schools:
                    type = "schools";
                    break;
                case Inter:
                    type = "intermediate colleges";
                    break;
                case Engineering:
                    type = "engineering colleges";
                    break;
                case Medical:
                    type = "medical colleges";
                    break;
                case Polytechnic:
                    type = "polytehnic colleges";
                    break;
                case Library:
                    type = "libraries";
                    break;
                case BookStores:
                    type = "book stores";
                    break;
            }
            TextSearch ts = new TextSearch();
            var attachment = ts.makeRequest(placename, type);
            Activity msg = (Activity)context.MakeMessage();
            msg.Recipient = msg.Recipient;
            msg.Type = "message";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = attachment;
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        public async Task ShoppingPrompt(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.GetShoppingPlaces,
                this.ShoppingPlaces,
                "What do you like to shop???",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task GetShoppingPlaces(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            var type = "";
            await context.PostAsync("You have selected " + selectedCard);
            switch (selectedCard)
            {
                case ShoppingMalls:
                    type = "shopping malls";
                    break;
                case JewelryStores:
                    type = "jewelry stores";
                    break;
                case MobileStores:
                    type = "mobile stores";
                    break;
                case CarLeasing:
                    type = "car leasing";
                    break;
            }
            TextSearch ts = new TextSearch();
            var attachment = ts.makeRequest(placename, type);
            Activity msg = (Activity)context.MakeMessage();
            msg.Recipient = msg.Recipient;
            msg.Type = "message";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = attachment;
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        public async Task SpaPrompt(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.GetSpaPlaces,
                this.SpaPlaces,
                "Which place do you like to choose???",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task GetSpaPlaces(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            var type = "";
            await context.PostAsync("You have selected " + selectedCard);
            switch (selectedCard)
            {
                case BeautyParlour:
                    type = "beauty parlours";
                    break;
                case Salons:
                    type = "salons";
                    break;
                case Gym:
                    type = "gyms";
                    break;
            }
            TextSearch ts = new TextSearch();
            var attachment = ts.makeRequest(placename, type);
            Activity msg = (Activity)context.MakeMessage();
            msg.Recipient = msg.Recipient;
            msg.Type = "message";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = attachment;
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }


        public async Task TourismPrompt(IDialogContext context)
        {
            PromptDialog.Choice<string>(
                context,
                this.GetTourismPlaces,
                this.TourismPlaces,
                "Which place do you like to choose???",
                "Ooops, what you wrote is not a valid option, please try again",
                3,
                PromptStyle.Auto);
        }

        public async Task GetTourismPlaces(IDialogContext context, IAwaitable<string> result)
        {
            var selectedCard = await result;
            var type = "";
            await context.PostAsync("You have selected " + selectedCard);
            switch (selectedCard)
            {
                case Museums:
                    type = "museums";
                    break;
                case Zoos:
                    type = "zoos";
                    break;
                case ArtGalleries:
                    type = "art galleries";
                    break;
                case Parks:
                    type = "parks";
                    break;
                case Resorts:
                    type = "resorts";
                    break;
                case FamousPlaces:
                    type = "famous places";
                    break;

            }
            TextSearch ts = new TextSearch();
            var attachment = ts.makeRequest(placename, type);
            Activity msg = (Activity)context.MakeMessage();
            msg.Recipient = msg.Recipient;
            msg.Type = "message";
            msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            msg.Attachments = attachment;
            await context.PostAsync(msg);
            context.Wait(MessageReceived);
        }

        [LuisIntent("AskDetails")]
        public async Task AskDetails(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"fetching details");
            EntityRecommendation rec;
            if (result.TryFindEntity("place", out rec))
            {
                placename = rec.Entity;
                await context.PostAsync($"place : " + placename);
                PlaceDetails pd = new PlaceDetails();
                var attachment = pd.displayPlaceDetails(placename);
                Activity msg = (Activity)context.MakeMessage();
                msg.Recipient = msg.Recipient;
                msg.Type = "message";
                msg.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                msg.Attachments.Add(attachment);
                await context.PostAsync(msg);
            }
            else
            {
                await context.PostAsync($"Please enter your place ");
            }

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