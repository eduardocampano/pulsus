using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.ApplicationPages;
using Microsoft.SharePoint;
using Pulsus.SharePoint.TimerJobs;
using Pulsus.SharePoint.Core;

namespace Pulsus.SharePoint.ApplicationPages
{
	public partial class PulsusSettings : DialogAdminPageBase
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}
		
		protected void Selector_ContextChange(object sender, EventArgs e)
		{
			var application = GetApplication();
			if (application.Properties.ContainsKey(Constants.PulsusConfigKey))
				configuration.Text = Convert.ToString(application.Properties[Constants.PulsusConfigKey]);
			else
			{
				var applicationConfig = application.GetPulsusConfigPath();
				if (File.Exists(applicationConfig))
					configuration.Text = File.ReadAllText(applicationConfig);
			}
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
				LabelErrorMessage.Text = "Invalid XML configuration";
				return;
			}

			if (application.Properties.ContainsKey(Constants.PulsusConfigKey))
				application.Properties[Constants.PulsusConfigKey] = configuration.Text;
			else
				application.Properties.Add(Constants.PulsusConfigKey, configuration.Text);

			var configDeploymentJob = new ConfigDeploymentJob(Constants.ConfigDeploymentJobName, application);
			var schedule = new SPOneTimeSchedule(DateTime.Now);
			configDeploymentJob.Schedule = schedule;
			configDeploymentJob.Update();

			foreach (SPJobDefinition job in application.JobDefinitions)
				if (job.Title == Constants.ConfigDeploymentJobName)
					job.Delete();

			application.JobDefinitions.Add(configDeploymentJob);
			application.Update();

			configDeploymentJob.RunNow();

			ReloadPulsusFeature(application);

			base.EndOperation(0);
		}

		protected void ReloadPulsusFeature(SPWebApplication application)
		{
			application.Features.Add(Constants.PulsusFeatureId, true);
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
