using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeANativeBot
{
    public class AboutCard
    {
        public Attachment generateHelpCard()
        {
            List<CardAction> buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.PostBack, " Suggest places around you ", value : "Suggest places"),
                        new CardAction(ActionTypes.PostBack, " Get details of a place ", value : "Get Details")
                    };
            var attachment = GetHeroCard("Now that you know about me, you can try asking me", buttons);
            return attachment;
        }

        private static Attachment GetHeroCard(string text, List<CardAction> cardAction)
        {
            var heroCard = new HeroCard
            {
                Text = text,
                Buttons = cardAction,
            };

            return heroCard.ToAttachment();
        }
    }
}