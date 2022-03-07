using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;

namespace QueueService
{
    public class Queue_Post : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity Target = (Entity)context.InputParameters["Target"];
                string serializedJson = JsonConvert.SerializeObject(Target);
                //string serializedJson = Target.ToString();
                Entity Queue = new Entity("mtx_queue");
                Queue["mtx_context"] = serializedJson;

                switch (Target.LogicalName)
                {
                    case "account":
                        Queue["mtx_entitytype"] = new OptionSetValue(866890000);
                        break;
                    case "contact":
                        Queue["mtx_entitytype"] = new OptionSetValue(866890001);
                        break;
                    case "lead":
                        Queue["mtx_entitytype"] = new OptionSetValue(866890002);
                        break;
                    default:
                        throw new InvalidPluginExecutionException("Invalid entity type" + Target.LogicalName);
                }
                Queue["mtx_maxnumberofattempts"] = 5;
                Queue["mtx_numberofattempts"] = 0;

                service.Create(Queue);

            }

        }
    }
}
