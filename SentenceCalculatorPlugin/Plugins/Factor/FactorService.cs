using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Text.Json;

namespace SentenceCalculatorPlugin.Plugins.Factor
{
    public class FactorService
    {
        private readonly IOrganizationService _service;
        private readonly ITracingService _tracing;

        private const string TABLE_NAME = "cr3e9_df_factors";

        public FactorService(IOrganizationService service, ITracingService tracing)
        {
            _service = service;
            _tracing = tracing;
        }

        public string GetFactorSummary()
        {
            _tracing.Trace("FactorService Started");

            try
            {
                // ================================
                // 1. TOTAL COUNT
                // ================================
                var allData = _service.RetrieveMultiple(new QueryExpression(TABLE_NAME)
                {
                    ColumnSet = new ColumnSet(false)
                });

                int totalCount = allData.Entities.Count;

                _tracing.Trace($"Total Count: {totalCount}");

                // ================================
                // 2. AGGRAVATING (df_factortype = 1)
                // ================================
                var aggravating = GetFactorsByType(1);

                // ================================
                // 3. MITIGATING (df_factortype = 2)
                // ================================
                var mitigating = GetFactorsByType(2);

                // ================================
                // FINAL JSON
                // ================================
                var result = new
                {
                    totalFactorCount = totalCount,
                    aggravatingFactors = aggravating,
                    mitigatingFactors = mitigating
                };

                string json = JsonSerializer.Serialize(result);

                _tracing.Trace("FactorService Completed Successfully");

                return json;
            }
            catch (Exception ex)
            {
                _tracing.Trace("Service Error: " + ex.ToString());
                throw new InvalidPluginExecutionException("FactorService Error: " + ex.Message);
            }
        }

        // ================================
        // 🔹 COMMON METHOD
        // ================================
        private object GetFactorsByType(int type)
        {
            QueryExpression query = new QueryExpression(TABLE_NAME)
            {
                ColumnSet = new ColumnSet(
                    "df_factorname",
                    "df_factorprimaryid",
                    "df_factortype",
                    "df_sequencenumber"
                )
            };

            query.Criteria.AddCondition("df_factortype", ConditionOperator.Equal, type);

            var data = _service.RetrieveMultiple(query);

            return data.Entities.Select(e => new
            {
                id = e.Id,
                name = e.GetAttributeValue<string>("df_factorname")
                       ?? e.GetAttributeValue<string>("df_factorprimaryid") // fallback
                       ?? string.Empty,

                description = e.GetAttributeValue<string>("df_factorprimaryid") ?? string.Empty,

                sequence = e.GetAttributeValue<int?>("df_sequencenumber") ?? 0
            }).ToList();
        }
    }
}
