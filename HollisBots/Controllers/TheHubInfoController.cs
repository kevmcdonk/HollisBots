using Autofac;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Internals.Fibers;
using HollisBots.Dialogs;

namespace HollisBots.Controllers
{
    [BotAuthentication(CredentialProviderType = typeof(MultiCredentialProvider))]
    [RoutePrefix("/api/HubInfoMessages")]
    public class HubInfoMessagesController : ApiController
    {
        private readonly ILifetimeScope scope;
        
        public HubInfoMessagesController(ILifetimeScope scope)
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
                    //await Conversation.SendAsync(activity, () => new Dialogs.RootHubInfoDialog());
                    using (var scope = DialogModule.BeginLifetimeScope(this.scope, activity))
                    {
                        //var postToBot = scope.Resolve<IPostToBot>();
                        //await postToBot.PostAsync(activity, token);

                        await Conversation.SendAsync(activity, () => scope.Resolve<IInfoDialog>());
                    }
                }
                else
                {
                    await HandleSystemMessage(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }
            catch (System.Exception exp)
            {
                System.Console.WriteLine("Hit an error in info controller: " + exp.Message + ";;;" + exp.StackTrace);
                throw exp;
            }
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.Event && message.Name == "initializeBot")
            {
                IConversationUpdateActivity conversationupdate = message;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    var reply = message.CreateReply();
                    reply.Text = "you asked: " + message.Value;
                    reply.ReplyToId = message.Recipient.Id;
                    await client.Conversations.ReplyToActivityAsync(reply);
                }
                
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                IConversationUpdateActivity conversationupdate = message;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (conversationupdate.MembersAdded.Any())
                    {
                        var reply = message.CreateReply();
                        foreach (var newMember in conversationupdate.MembersAdded)
                        {
                            if (newMember.Id != message.Recipient.Id)
                            {
                                reply.Text = $"Welcome to the info bot {newMember.Name}! ";
                            }
                            else
                            {
                                reply.Text = $"Welcome to the info bot {message.From.Name}";
                            }
                            reply.ReplyToId = message.Recipient.Id;
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
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