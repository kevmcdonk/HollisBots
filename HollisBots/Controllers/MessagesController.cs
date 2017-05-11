using Autofac;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace HollisBots.Controllers
{
    [BotAuthentication(CredentialProviderType = typeof(MultiCredentialProvider))]
    public class MessagesController : ApiController
    {
        private readonly ILifetimeScope scope;

        public MessagesController(ILifetimeScope scope)
        {
            SetField.NotNull(out this.scope, nameof(scope), scope);

            // Update the container to use the right MicorosftAppCredentials based on
            // Identity set by BotAuthentication
            var builder = new ContainerBuilder();

            builder.Register(c => ((ClaimsIdentity)HttpContext.Current.User.Identity).GetCredentialsFromClaims())
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.Update(Conversation.Container);
        }
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken token)
        {
            try
            {
                if (activity.Type == ActivityTypes.Message)
                {
                    using (var scope = DialogModule.BeginLifetimeScope(this.scope, activity))
                    {
                        var postToBot = scope.Resolve<IPostToBot>();
                        await postToBot.PostAsync(activity, token);
                    }
                }
                else
                {
                    HandleSystemMessage(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            } catch (System.Exception exp)
            {
                System.Console.WriteLine("Hit an error in Messages Controller: " + exp.Message + ";;;" + exp.StackTrace);
                throw exp;
            }
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