using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace HollisBots.Dialogs
{
    public interface IPersonDialog : IDialog<object>
    {
        Task None(IDialogContext context, LuisResult result);
        Task SearchPersonTask(IDialogContext context, LuisResult result);
    }
}