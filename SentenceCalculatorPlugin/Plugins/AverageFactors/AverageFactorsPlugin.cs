using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceCalculatorPlugin.Plugins.AverageFactors
{
    public  class AverageFactorsPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            // =========================
            // 1. Fetch Data
            // =========================
            QueryExpression query = new QueryExpression("cr3e9_df_sentencefactor")
            {
                ColumnSet = new ColumnSet(
                    "cr3e9_df_sentencecustomfactorname",
                    "cr3e9_df_changeinsentence",
                    "cr3e9_df_sentencecustomfactortype"
                )
            };

            var result = service.RetrieveMultiple(query);

            // =========================
            // 2. Aggravating / Mitigating AVG
            // =========================
            var aggravating = result.Entities
                .Where(x => x.GetAttributeValue<OptionSetValue>("cr3e9_df_sentencecustomfactortype")?.Value == 1)
                .GroupBy(x => x.GetAttributeValue<string>("cr3e9_df_sentencecustomfactorname"))
                .Select(g => new
                {
                    factorName = g.Key,
                    average = Math.Round(g.Average(x => (decimal)(x.GetAttributeValue<int?>("cr3e9_df_changeinsentence") ?? 0)), 2)
                })
                .ToList();

            var mitigating = result.Entities
                .Where(x => x.GetAttributeValue<OptionSetValue>("cr3e9_df_sentencecustomfactortype")?.Value == 2)
                .GroupBy(x => x.GetAttributeValue<string>("cr3e9_df_sentencecustomfactorname"))
                .Select(g => new
                {
                    factorName = g.Key,
                    average = Math.Round(g.Average(x => (decimal)(x.GetAttributeValue<int?>("cr3e9_df_changeinsentence") ?? 0)), 2)
                })
                .ToList();

            // =========================
            // 3. Categories from Enum
            // =========================
            var categories = Enum.GetValues(typeof(Category))
                .Cast<Category>()
                .Select(c => new
                {
                    id = (int)c - 1, // 👈 JSON me 0-based chahiye
                name = c.ToString()
                })
                .ToList();

            // =========================
            // 4. Final Response
            // =========================
            var response = new
            {
                totalFactorCount = result.Entities.Count,
                categories = categories,
                aggravatingAverageFactors = aggravating,
                mitigatingAverageFactors = mitigating
            };

            // =========================
            // 5. Return JSON
            // =========================
            context.OutputParameters["response"] = JsonConvert.SerializeObject(response);
        }
    }

}
