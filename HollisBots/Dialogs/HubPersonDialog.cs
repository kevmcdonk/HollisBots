using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace TheHubBots.Dialogs
{
    [Serializable]
    public class RootHubPersonDialog : LuisDialog<object>
    {
        private readonly ILuisService luis;

        public RootHubPersonDialog(ILuisService luis)
            : base(luis)
        {
            SetField.NotNull(out this.luis, nameof(luis), luis);
        }

        [LuisIntent("SearchPerson")]
        public async Task SearchPersonTask(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Here is your person details.");
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