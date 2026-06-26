using Microsoft.Xrm.Sdk;
using System;
namespace SentenceCalculatorPlugin.Plugins.SentenceCreate
{
    public class CreateSentencePlugin : IPlugin
    {
        // ✅ Handles int, double, decimal, string
        private decimal SafeDecimal(object value)
        {
            if (value == null) return 0;
            try { return Convert.ToDecimal(value); }
            catch { return 0; }
        }

        private int SafeInt(object value)
        {
            if (value == null) return 0;
            try { return Convert.ToInt32(value); }
            catch { return 0; }
        }

        private OptionSetValue SafeOptionSet(object value)
        {
            if (value == null) return null;
            try { return new OptionSetValue(Convert.ToInt32(value)); }
            catch { return null; }
        }

        private DateTime SafeDate(object value)
        {
            if (value == null) return DateTime.Now;

            DateTime dt;
            return DateTime.TryParse(value.ToString(), out dt) ? dt : DateTime.Now;
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            try
            {
                // =========================
                // SAFE INPUT
                // =========================

                decimal quantity = SafeDecimal(context.InputParameters["df_quantitydetained"]);
                decimal quantityGram = SafeDecimal(context.InputParameters["df_quantitydetainedingram"]);
                decimal percentage = SafeDecimal(context.InputParameters["df_drugquantitypercentage"]);

                int age = SafeInt(context.InputParameters["df_age"]);

                OptionSetValue unit = SafeOptionSet(context.InputParameters["df_unit"]);
                OptionSetValue quantityType = SafeOptionSet(context.InputParameters["df_quantitytype"]);
                OptionSetValue gender = SafeOptionSet(context.InputParameters["df_gender"]);

                DateTime confiscationDate = SafeDate(context.InputParameters["df_confiscationdate"]);

                int sentenceDays = SafeInt(context.InputParameters["df_sentencedays"]);
                decimal fine = SafeDecimal(context.InputParameters["df_fine"]);

                string sentenceFormat = context.InputParameters.Contains("df_sentenceyymmdd")
                    ? Convert.ToString(context.InputParameters["df_sentenceyymmdd"])
                    : string.Empty;

                // =========================
                // CREATE ENTITY
                // =========================

                var entity = new Entity("df_sentence");

                // ✅ Decimal fields
                entity["df_quantitydetained"] = quantity;
                entity["df_quantitydetainedingram"] = quantityGram;
                entity["df_drugquantitypercentage"] = percentage;

                // ✅ Whole number
                entity["df_age"] = age;
                entity["df_sentencedays"] = sentenceDays;
                entity["df_fine"] = fine;

                // ✅ Choice fields
                entity["df_unit"] = unit;
                entity["df_quantitytype"] = quantityType;
                entity["df_gender"] = gender;

                // ✅ Date
                entity["df_confiscationdate"] = confiscationDate;

                // ✅ Text
                entity["df_sentenceyymmdd"] = sentenceFormat;

                // =========================
                // SAVE
                // =========================

                Guid recordId = service.Create(entity);

                context.OutputParameters["message"] = "Record created successfully.";
                context.OutputParameters["id"] = recordId;
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(
                    "CreateSentencePlugin failed: " + ex.Message
                );
            }
        }
    }
}
