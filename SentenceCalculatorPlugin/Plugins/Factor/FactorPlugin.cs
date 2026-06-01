using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceCalculatorPlugin.Plugins.Factor
{
    public class FactorPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);
            var tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracing.Trace("Plugin Started");

            try
            {
                // ✅ Call Service Class
                FactorService factorService = new FactorService(service, tracing);
                string jsonResult = factorService.GetFactorSummary();

                // ✅ Set Output Parameter
                context.OutputParameters["factorSummary"] = jsonResult;

                tracing.Trace("factorSummary set successfully");
                tracing.Trace("Plugin Completed Successfully");
            }
            catch (Exception ex)
            {
                tracing.Trace("Plugin Error: " + ex.ToString());
                throw new InvalidPluginExecutionException("Error: " + ex.Message);
            }
        }
    }
}
