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

                double quantity = context.InputParameters.Contains("df_quantitydetained")
                    ? Convert.ToDouble(context.InputParameters["df_quantitydetained"])
                    : 0.0;

                double quantityGram = context.InputParameters.Contains("df_quantitydetainedingram")
                    ? Convert.ToDouble(context.InputParameters["df_quantitydetainedingram"])
                    : 0.0;

                double unit = context.InputParameters.Contains("df_unit")
                    ? Convert.ToDouble(context.InputParameters["df_unit"])
                    : 0.0;

                DateTime confiscationDate = context.InputParameters.Contains("df_confiscationdate")
                    ? Convert.ToDateTime(context.InputParameters["df_confiscationdate"])
                    : DateTime.Now;

                double percentage = context.InputParameters.Contains("df_drugquantitypercentage")
                    ? Convert.ToDouble(context.InputParameters["df_drugquantitypercentage"])
                    : 0.0;

                double age = context.InputParameters.Contains("df_age")
                    ? Convert.ToDouble(context.InputParameters["df_age"])
                    : 0.0;

                double gender = context.InputParameters.Contains("df_gender")
                    ? Convert.ToDouble(context.InputParameters["df_gender"])
                    : 0.0;

                double sentenceDays = context.InputParameters.Contains("df_sentencedays")
                    ? Convert.ToDouble(context.InputParameters["df_sentencedays"])
                    : 0.0;

                double fine = context.InputParameters.Contains("df_fine")
                    ? Convert.ToDouble(context.InputParameters["df_fine"])
                    : 0.0;

                string sentenceFormat = context.InputParameters.Contains("df_sentenceyymmdd")
                    ? context.InputParameters["df_sentenceyymmdd"].ToString()
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

                // ✅ FIXED: Whole Number field = int (NOT Money, NOT decimal)
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
                throw new InvalidPluginExecutionException("CreateSentencePlugin failed: " + ex.Message);
            }
        }
    }
}
