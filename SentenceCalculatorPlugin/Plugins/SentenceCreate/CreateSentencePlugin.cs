using Microsoft.Xrm.Sdk;
using System;
namespace SentenceCalculatorPlugin.Plugins.SentenceCreate
{
    public class CreateSentencePlugin : IPlugin
    {
        private double SafeDouble(object value)
        {
            if (value == null) return 0.0;

            try
            {
                return Convert.ToDouble(value);
            }
            catch
            {
                return 0.0;
            }
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

                double quantity = SafeDouble(context.InputParameters["df_quantitydetained"]);
                double quantityGram = SafeDouble(context.InputParameters["df_quantitydetainedingram"]);
                double unit = SafeDouble(context.InputParameters["df_unit"]);

                DateTime confiscationDate = SafeDate(context.InputParameters["df_confiscationdate"]);

                double percentage = SafeDouble(context.InputParameters["df_drugquantitypercentage"]);
                double age = SafeDouble(context.InputParameters["df_age"]);
                double gender = SafeDouble(context.InputParameters["df_gender"]);
                double sentenceDays = SafeDouble(context.InputParameters["df_sentencedays"]);
                double fine = SafeDouble(context.InputParameters["df_fine"]);

                // ✅ FIXED STRING FIELD
                string sentenceFormat = context.InputParameters.Contains("df_sentenceyymmdd")
                    ? Convert.ToString(context.InputParameters["df_sentenceyymmdd"])
                    : string.Empty;

                // =========================
                // CREATE ENTITY
                // =========================

                var entity = new Entity("cr3e9_df_sentences");

                entity["cr3e9_df_quantitydetained"] = quantity;
                entity["cr3e9_df_quantitydetainedingram"] = quantityGram;
                entity["cr3e9_df_unit"] = unit;
                entity["cr3e9_df_confiscationdate"] = confiscationDate;

                entity["cr3e9_df_drugquantitypercentage"] = percentage;
                entity["cr3e9_df_age"] = age;
                entity["cr3e9_df_gender"] = gender;

                entity["cr3e9_df_sentencedays"] = sentenceDays;

                // fine (adjust based on Dataverse field type)
                entity["cr3e9_df_fine"] = Convert.ToInt32(Math.Round(fine));

                entity["cr3e9_df_sentenceyymmdd"] = sentenceFormat;

                // =========================
                // SAVE
                // =========================
                Guid recordId = service.Create(entity);

                // =========================
                // OUTPUT
                // =========================
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
