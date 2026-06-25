using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
namespace SentenceCalculatorPlugin.Plugins.Factor
{

    public class FactorListRetrievalPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context =
                (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service =
                serviceFactory.CreateOrganizationService(context.UserId);

            ITracingService tracing =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracing.Trace("FactorListRetrievalPlugin started.");

            try
            {
                string entityName = "df_factor";

                // Step 1: Query data from Dataverse table
                QueryExpression query = new QueryExpression("df_factor")
                {
                    ColumnSet = new ColumnSet(true)
                };


                EntityCollection result = service.RetrieveMultiple(query);

                tracing.Trace("Records found: " + result.Entities.Count);

                // IMPORTANT FIX (same as your working drug plugin)
                result.EntityName = entityName;

                // MUST match Custom API response property name
                context.OutputParameters["factorSummaryList"] = result;

                tracing.Trace("Output parameter 'factorSummaryList' set successfully.");
                tracing.Trace("FactorListRetrievalPlugin completed successfully.");
            }
            catch (Exception ex)
            {
                tracing.Trace("Exception: " + ex.ToString());

                throw new InvalidPluginExecutionException(
                    "Error in FactorListRetrievalPlugin: " + ex.Message);
            }
        }
    }
}
