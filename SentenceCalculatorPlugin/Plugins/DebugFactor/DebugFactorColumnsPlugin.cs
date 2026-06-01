using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace SentenceCalculatorPlugin.Plugins.DebugFactor
{
    public class DebugFactorColumnsPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            tracing.Trace("=== DEBUG PLUGIN START ===");

            try
            {
                tracing.Trace("Creating Query...");

                QueryExpression query = new QueryExpression("cr3e9_df_factors")
                {
                    ColumnSet = new ColumnSet(true),
                    TopCount = 5
                };

                tracing.Trace("Executing RetrieveMultiple...");

                EntityCollection data = service.RetrieveMultiple(query);

                tracing.Trace("Records Retrieved: " + data.Entities.Count);

                foreach (Entity entity in data.Entities)
                {
                    tracing.Trace("---- RECORD START ----");

                    foreach (var attr in entity.Attributes)
                    {
                        tracing.Trace("Column: " + attr.Key);
                    }

                    tracing.Trace("---- RECORD END ----");
                }

                tracing.Trace("=== DEBUG SUCCESS ===");

                throw new InvalidPluginExecutionException("Check Plugin Trace Logs");
            }
            catch (Exception ex)
            {
                // 🔥 MOST IMPORTANT PART
                tracing.Trace("=== ERROR START ===");
                tracing.Trace("Message: " + ex.Message);
                tracing.Trace("StackTrace: " + ex.StackTrace);

                if (ex.InnerException != null)
                {
                    tracing.Trace("Inner Exception: " + ex.InnerException.Message);
                }

                tracing.Trace("=== ERROR END ===");

                //throw new InvalidPluginExecutionException("REAL ERROR: " + ex.Message);
            }
        }
    }
}
