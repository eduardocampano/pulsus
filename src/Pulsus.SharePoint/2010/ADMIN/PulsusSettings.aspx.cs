using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.ApplicationPages;

namespace Pulsus.SharePoint.ApplicationPages
{
    public partial class PulsusSettings : DialogAdminPageBase
    {
        private static readonly Guid PulsusFeatureId = new Guid("7465ed3a-f5cc-46ba-8205-0ba5fc0e8b35");

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        
        protected void Selector_ContextChange(object sender, EventArgs e)
        {
            var application = GetApplication();
            var applicationConfig = GetApplicationConfig(application);
            if (File.Exists(applicationConfig))
                configuration.Text = File.ReadAllText(applicationConfig);
        }

        protected void okButton_Click(object sender, EventArgs e)
        {
            var application = GetApplication();

            try
            {
                var xDocument = XDocument.Parse(configuration.Text);
            }
            catch (Exception)
            {
                // TODO: show error
                LabelErrorMessage.Text = "Invalid XML condfiguration";
                return;
            }
            
            Enum.GetValues(typeof(SPUrlZone)).Cast<SPUrlZone>().ToList().ForEach(z =>
            {
                var applicationConfig = GetApplicationConfig(application);
                File.WriteAllText(applicationConfig, configuration.Text);
            });

            ReloadPulsusFeature(application);
            
            base.EndOperation(0);
        }

        protected void ReloadPulsusFeature(SPWebApplication application)
        {
            application.Features.Add(PulsusFeatureId, true);
        }

        protected string GetApplicationConfig(SPWebApplication application)
        {
            return Path.Combine(application.GetIisSettingsWithFallback(SPUrlZone.Default).Path.FullName, "Pulsus.config");
        }

        protected SPWebApplication GetApplication()
        {
            var currentApplicationId = new Guid(Selector.CurrentId);
            var application = SPWebService.ContentService.WebApplications.FirstOrDefault(a => a.Id.Equals(currentApplicationId));
            if (application == null)
                throw new Exception("Could not find application " + currentApplicationId);

            return application;
        }
    }
}
