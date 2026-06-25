using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SentenceCalculatorPlugin.Model;
using System;
namespace SentenceCalculatorPlugin.Plugins.Marquee
{
    public class MarqueeSettingsRetrievalPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Context
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Service Factory
            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                serviceFactory.CreateOrganizationService(context.UserId);

            ITracingService tracing =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
                tracing.Trace("MarqueeSettingsRetrievalPlugin execution started.");

                // TODO: Replace with your actual table logical name
                string entityName = "df_setting";

                QueryExpression query = new QueryExpression(entityName)
                {
                    ColumnSet = new ColumnSet(
                     "df_marqueelink",
                     "df_marqueelinktext",
                     "df_marqueetext",
                     "df_marqueevisible"),
                    TopCount = 1
                };

                query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));

                EntityCollection result = service.RetrieveMultiple(query);

                if (result.Entities.Count == 0)
                {
                    tracing.Trace("No marquee records found.");
                    context.OutputParameters["marquee"] = null;
                    return;
                }

                Entity marqueeRecord = result.Entities[0];

                Entity output = new Entity();

                string link =marqueeRecord.GetAttributeValue<string>("df_marqueelink") ?? string.Empty;

                string linkText =marqueeRecord.GetAttributeValue<string>("df_marqueelinktext") ?? string.Empty;

                string text =marqueeRecord.GetAttributeValue<string>("df_marqueetext") ?? string.Empty;

                bool visible =marqueeRecord.GetAttributeValue<bool?>("df_marqueevisible") ?? false;

                // assign output
                output["marqueelink"] = link;
                output["marqueelinktext"] = linkText;
                output["marqueetext"] = text;
                output["marqueevisible"] = visible;

                // return
                context.OutputParameters["marquee"] = output;

                tracing.Trace("Marquee record returned successfully.");
            }
            catch (Exception ex)
            {
                tracing.Trace("Error: " + ex.ToString());
                throw new InvalidPluginExecutionException(
                    "Error in MarqueeSettingsRetrievalPlugin: " + ex.Message);
            }
        }
    }
}
