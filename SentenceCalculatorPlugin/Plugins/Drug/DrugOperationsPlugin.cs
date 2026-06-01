using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceCalculatorPlugin.Plugins.Drug
{
    public class DrugOperationsPlugin : IPlugin
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

            tracing.Trace("GetDrugList Plugin Started");

            try
            {
                // Step 1: Query data from Dataverse table
                QueryExpression query = new QueryExpression("cr3e9_df_drugs")
                {
                    ColumnSet = new ColumnSet(true)
                };

                EntityCollection drugList = service.RetrieveMultiple(query);

                tracing.Trace($"Records fetched: {drugList.Entities.Count}");

                // IMPORTANT: Ensure EntityName is set
                drugList.EntityName = "cr3e9_df_drugs";

                // Step 2: Set Custom API response parameter (FIXED)
                context.OutputParameters["df_drugs"] = drugList;

                tracing.Trace("Output parameter 'df_drugs' set successfully");
                tracing.Trace("GetDrugList Plugin Completed Successfully");
            }
            catch (Exception ex)
            {
                tracing.Trace("Exception: " + ex.ToString());
                throw new InvalidPluginExecutionException("Error in GetDrugList Plugin: " + ex.Message);
            }
        }
    }
}
