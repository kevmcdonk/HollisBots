using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Configuration;
using HollisBots.Dialogs;

namespace HollisBots.Models
{
    public class HubModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            NameValueCollection section =
        (NameValueCollection)ConfigurationManager.GetSection("LuisList");
            foreach (string luisId in section.AllKeys)
            {
                builder.Register(c => new LuisModelAttribute(luisId, section[luisId])).AsSelf().AsImplementedInterfaces().SingleInstance();
            }

            
            builder.RegisterType<RootHubPersonDialog>().As<IPersonDialog>().InstancePerDependency();
            builder.RegisterType<RootHubNewsDialog>().As<INewsDialog>().InstancePerDependency();
            builder.RegisterType<RootDialog>().As<IDialog<object>>().InstancePerDependency();
            builder.RegisterType<RootHubInfoDialog>().As<IInfoDialog>().InstancePerDependency();

            // register some singleton services
            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StrictEntityToType>().Keyed<IEntityToType>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<LuisService>().Keyed<ILuisService>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ResolutionParser>().Keyed<IResolutionParser>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<WesternCalendarPlus>().Keyed<ICalendarPlus>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<StrictEntityToType>().Keyed<IEntityToType>(FiberModule.Key_DoNotSerialize).AsImplementedInterfaces().SingleInstance();


            // register the top level dialog
            builder.RegisterType<RootDialog>().As<LuisDialog<object>>().InstancePerDependency();
            builder.RegisterType<RootHubInfoDialog>().As<LuisDialog<object>>().InstancePerDependency();
            builder.RegisterType<RootHubNewsDialog>().As<LuisDialog<object>>().InstancePerDependency();
            builder.RegisterType<RootHubPersonDialog>().As<LuisDialog<object>>().InstancePerDependency();

            
        }
    }
}