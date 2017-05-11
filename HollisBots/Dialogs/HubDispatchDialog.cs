using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Scorables;

namespace TheHubBots.Dialogs
{
    /// <summary>
    /// The top-level natural language dialog for the alarm sample.
    /// </summary>
    [Serializable]
    [LuisModel("unitTestMockReturnedFromMakeService", "unitTestMockReturnedFromMakeService")]
    public sealed class HubDispatchDialog : DispatchDialog
    {
        
        private readonly IEntityToType entityToType;
        private readonly ILuisService luis;
        
        public HubDispatchDialog(IEntityToType entityToType, ILuisService luis)
        {
            SetField.NotNull(out this.entityToType, nameof(entityToType), entityToType);
            SetField.NotNull(out this.luis, nameof(luis), luis);
        }

        protected override ILuisService MakeService(ILuisModel model)
        {
            return this.luis;
        }

        [LuisIntent("builtin.intent.none")]
        // ScorableOrder allows the user to override the scoring process to create
        // ordered scorable groups, where the scores from the first scorable group
        // are compared first, and if there is no scorable that wishes to participate
        // from the first scorable group, then the second scorable group is considered, and so forth.
        // You might use this to ensure that regular expression scorables are considered
        // before LUIS intent scorables.
        [ScorableGroup(1)]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(ActivityReceivedAsync);
        }
        
    }
}