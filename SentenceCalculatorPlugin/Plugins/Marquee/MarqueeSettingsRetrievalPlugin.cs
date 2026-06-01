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
                string entityName = "cr3e9_df_settings";

                QueryExpression query = new QueryExpression(entityName)
                {
                    ColumnSet = new ColumnSet(
                     "cr3e9_df_marqueelink",
                     "cr3e9_df_marqueelinktext",
                     "cr3e9_df_marqueetext",
                     "cr3e9_df_marqueevisible"),
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

                string link =marqueeRecord.GetAttributeValue<string>("cr3e9_df_marqueelink") ?? string.Empty;

                string linkText =marqueeRecord.GetAttributeValue<string>("cr3e9_df_marqueelinktext") ?? string.Empty;

                string text =marqueeRecord.GetAttributeValue<string>("cr3e9_df_marqueetext") ?? string.Empty;

                bool visible =marqueeRecord.GetAttributeValue<bool?>("cr3e9_df_marqueevisible") ?? false;

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
