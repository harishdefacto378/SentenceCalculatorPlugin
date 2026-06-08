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
                // INPUT SAFE READ
                int quantity = Convert.ToInt32(context.InputParameters["df_quantitydetained"]);
                int quantityGram = Convert.ToInt32(context.InputParameters["df_quantitydetainedingram"]);
                int unit = Convert.ToInt32(context.InputParameters["df_unit"]);

                DateTime confiscationDate = (DateTime)context.InputParameters["df_confiscationdate"];

                Guid drugId = (Guid)context.InputParameters["df_drugid"];

                decimal percentage = Convert.ToDecimal(context.InputParameters["df_drugquantitypercentage"]);
                int age = Convert.ToInt32(context.InputParameters["df_age"]);
                int gender = Convert.ToInt32(context.InputParameters["df_gender"]);

                int sentenceDays = Convert.ToInt32(context.InputParameters["df_sentencedays"]);
                decimal fine = Convert.ToDecimal(context.InputParameters["df_fine"]);

                string sentenceFormat = context.InputParameters.Contains("df_sentenceyymmdd")
                    ? context.InputParameters["df_sentenceyymmdd"].ToString()
                    : "";

                // CREATE ENTITY
                var entity = new Entity("df_sentences");

                entity["df_quantitydetained"] = quantity;
                entity["df_quantitydetainedingram"] = quantityGram;
                entity["df_unit"] = unit;
                entity["df_confiscationdate"] = confiscationDate;

                entity["df_drugquantitypercentage"] = percentage;
                entity["df_age"] = age;
                entity["df_gender"] = gender;

                entity["df_sentencedays"] = sentenceDays;
                entity["df_fine"] = new Money(fine);

                entity["df_sentenceyymmdd"] = sentenceFormat;

                // LOOKUP
                entity["df_drugid"] = new EntityReference("df_drugs", drugId);

                // SAVE
                Guid recordId = service.Create(entity);

                // OUTPUT
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
