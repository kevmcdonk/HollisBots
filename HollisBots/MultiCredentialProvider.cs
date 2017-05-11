using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Collections.Specialized;
using System.Configuration;

namespace TheHubBots
{
    public class MultiCredentialProvider : ICredentialProvider
    {
        public Dictionary<string, string> Credentials = GetBotList();

        public static Dictionary<string, string> GetBotList()
        {
            NameValueCollection section =
        (NameValueCollection)ConfigurationManager.GetSection("BotList");
            Dictionary<string, string> botList = new Dictionary<string, string>();
            foreach(string botName in section.AllKeys)
            {
                botList.Add(botName, section[botName]);
            }
            return botList;
        }

        public Task<bool> IsValidAppIdAsync(string appId)
            {
                return Task.FromResult(this.Credentials.ContainsKey(appId));
            }

            public Task<string> GetAppPasswordAsync(string appId)
            {
                return Task.FromResult(this.Credentials.ContainsKey(appId) ? this.Credentials[appId] : null);
            }

            public Task<bool> IsAuthenticationDisabledAsync()
            {
                return Task.FromResult(!this.Credentials.Any());
            }
        }
    }