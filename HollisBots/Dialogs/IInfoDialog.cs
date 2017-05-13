using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace HollisBots.Dialogs
{
    public interface IInfoDialog : IDialog<object>
    {
        Task None(IDialogContext context, LuisResult result);
        Task SearchInfoTask(IDialogContext context, LuisResult result);
    }
}