using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace SentenceCalculatorPlugin.Plugins.AverageFactors
{
    public class AverageFactorsPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            // =========================
            // FETCH DATA
            // =========================
            var query = new QueryExpression("cr3e9_df_factors")
            {
                ColumnSet = new ColumnSet(
                    "cr3e9_df_factorname",
                    "cr3e9_df_sequencenumber",
                    "cr3e9_df_factortype"
                )
            };

            var result = service.RetrieveMultiple(query);

            // =========================
            // HELPERS
            // =========================
            int? GetType(Entity e)
            {
                var val = e.GetAttributeValue<object>("cr3e9_df_factortype");

                if (val is OptionSetValue os) return os.Value;
                if (val is int i) return i;
                return null;
            }

            decimal GetDecimal(Entity e, string field)
            {
                var val = e.GetAttributeValue<object>(field);

                if (val is int i) return i;
                if (val is decimal d) return d;
                if (val is double db) return (decimal)db;

                return 0;
            }

            string SafeString(Entity e, string field)
            {
                return e.GetAttributeValue<string>(field) ?? "Unknown";
            }

            // =========================
            // AGGRAVATING
            // =========================
            var aggravating = result.Entities
                .Where(x => GetType(x) == 1)
                .GroupBy(x => SafeString(x, "cr3e9_df_factorname"))
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new FactorAverageDto
                {
                    factorName = g.Key,
                    average = Math.Round(g.Average(x => GetDecimal(x, "cr3e9_df_sequencenumber")), 2)
                })
                .ToList();

            // =========================
            // MITIGATING
            // =========================
            var mitigating = result.Entities
                .Where(x => GetType(x) == 2)
                .GroupBy(x => SafeString(x, "cr3e9_df_factorname"))
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new FactorAverageDto
                {
                    factorName = g.Key,
                    average = Math.Round(g.Average(x => GetDecimal(x, "cr3e9_df_sequencenumber")), 2)
                })
                .ToList();

            // =========================
            // CATEGORIES ENUM
            // =========================
            var categories = Enum.GetValues(typeof(Category))
                .Cast<Category>()
                .Select(c => new CategoryDto
                {
                    id = (int)c,
                    name = c.ToString()
                })
                .ToList();

            // =========================
            // RESPONSE MODEL
            // =========================
            var response = new FactorSummaryResponse
            {
                totalFactorCount = result.Entities.Count,
                categories = categories,
                aggravatingAverageFactors = aggravating,
                mitigatingAverageFactors = mitigating
            };

            // =========================
            // IMPORTANT FIX (Dataverse SAFE)
            // =========================
            // =========================
            // ✅ FIXED OUTPUT (Dataverse Best Practice)
            // =========================

            // Direct value
            context.OutputParameters["totalFactorCount"] = response.totalFactorCount;

            // Convert lists to JSON string
            context.OutputParameters["categories"] = JsonConvert.SerializeObject(response.categories);

            context.OutputParameters["aggravatingAverageFactors"] = JsonConvert.SerializeObject(response.aggravatingAverageFactors);

            context.OutputParameters["mitigatingAverageFactors"] = JsonConvert.SerializeObject(response.mitigatingAverageFactors);
        }
    }
}
