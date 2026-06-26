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
                // ✅ Step 1: Query only required columns (FIXED)
                QueryExpression query = new QueryExpression("df_drug")
                {
                    ColumnSet = new ColumnSet(
                        "df_drugidentifier",
                        "df_drugtype",
                        "df_smallquantitygram",
                        "df_commercialquantitygram",
                       // "df_commercialmaxquantitygram",
                        "df_smallminsent",
                        "df_smallmaxsent",
                        "df_interminsent",
                        "df_intermaxsent",
                        "df_commminsent",
                        "df_commmaxsent",
                        "df_smallminfine",
                        "df_smallmaxfine",
                        "df_interminfine",
                        "df_intermaxfine",
                        "df_commminfine",
                        "df_commmaxfine",
                        "df_punishableundersectionsmall",
                        "df_punishableundersectionintermediate",
                        "df_punishableundersectioncommercial",
                        "df_notificationno_under_viia_xxiiia_of_s2",
                        "df_notificationdate_under_viia_xxiiia_of_s2",
                        "df_notificationreportanddate",
                        "df_chemicalname_defined_in_s2xxiii"
                    )
                };

                EntityCollection drugList = service.RetrieveMultiple(query);

                tracing.Trace($"Records fetched: {drugList.Entities.Count}");

                // ✅ Ensure EntityName is set
                drugList.EntityName = "df_drug";

                // ✅ Step 2: Set Custom API response
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
