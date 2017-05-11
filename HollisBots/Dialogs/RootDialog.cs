using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Internals.Fibers;
using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace TheHubBots.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private readonly IEntityToType entityToType;
        

        public RootDialog(IEntityToType entityToType, ILuisService luis)
            : base(luis)
        {
            SetField.NotNull(out this.entityToType, nameof(entityToType), entityToType);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I will ask the @HubHelpBot: " + context.Activity.AsMessageActivity().Text);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SearchNews")]
        public async Task SearchNewsTask(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I will ask the @HubNewsBot: " + context.Activity.AsMessageActivity().Text);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SearchPerson")]
        public async Task SearchPersonTask(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I will ask the @HubPersonBot: " + context.Activity.AsMessageActivity().Text);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SearchInformation")]
        public async Task SearchInfoTask(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I will ask the @HubInfoBot: " + context.Activity.AsMessageActivity().Text);
            context.Wait(MessageReceived);
        }
        
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = "Sorry I did not understand.";
            IntentRecommendation bestBet =  this.BestIntentFrom(result);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        
    }
}