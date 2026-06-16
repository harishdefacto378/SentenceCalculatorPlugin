using Microsoft.Xrm.Sdk;
using System;
namespace SentenceCalculatorPlugin.Plugins.SentenceCreate
{
    public class CreateSentencePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var service = factory.CreateOrganizationService(context.UserId);

            try
            {
                // =========================
                // SAFE INPUT (DOUBLE for Dataverse number fields)
                // =========================

                // =========================
                // SAFE INPUT (UPDATED BASED ON REQUEST PAYLOAD)
                // =========================

                int quantity = context.InputParameters.Contains("df_quantitydetained")
                    ? Convert.ToInt32(context.InputParameters["df_quantitydetained"])
                    : 0;

                int quantityGram = context.InputParameters.Contains("df_quantitydetainedingram")
                    ? Convert.ToInt32(context.InputParameters["df_quantitydetainedingram"])
                    : 0;

                int unit = context.InputParameters.Contains("df_unit")
                    ? Convert.ToInt32(context.InputParameters["df_unit"])
                    : 0;

                // Date FIX (safe parsing for "2026-06-08")
                DateTime confiscationDate = context.InputParameters.Contains("df_confiscationdate")
                    ? DateTime.Parse(context.InputParameters["df_confiscationdate"].ToString())
                    : DateTime.Now;

                double percentage = context.InputParameters.Contains("df_drugquantitypercentage")
                    ? Convert.ToDouble(context.InputParameters["df_drugquantitypercentage"])
                    : 0.0;

                int age = context.InputParameters.Contains("df_age")
                    ? Convert.ToInt32(context.InputParameters["df_age"])
                    : 0;

                int gender = context.InputParameters.Contains("df_gender")
                    ? Convert.ToInt32(context.InputParameters["df_gender"])
                    : 0;

                int sentenceDays = context.InputParameters.Contains("df_sentencedays")
                    ? Convert.ToInt32(context.InputParameters["df_sentencedays"])
                    : 0;

                // fine: based on your payload (4000 is integer)
                int fine = context.InputParameters.Contains("df_fine")
                    ? Convert.ToInt32(context.InputParameters["df_fine"])
                    : 0;

                string sentenceFormat = context.InputParameters.Contains("df_sentenceyymmdd")
                    ? context.InputParameters["df_sentenceyymmdd"].ToString()
                    : string.Empty;

                // Optional field (IMPORTANT from your payload)
                int multiplier = context.InputParameters.Contains("df_multiplierforcommerical")
                    ? Convert.ToInt32(context.InputParameters["df_multiplierforcommerical"])
                    : 0;

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

                // ✅ FIXED: Whole Number field = int (NOT Money, NOT decimal)
                entity["cr3e9_df_fine"] = Convert.ToInt32(fine);

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
                throw new InvalidPluginExecutionException("CreateSentencePlugin failed: " + ex.Message);
            }
        }
    }
}
