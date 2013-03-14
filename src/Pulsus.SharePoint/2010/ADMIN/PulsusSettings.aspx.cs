using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;
using System.Reflection;
using Microsoft.SharePoint.Administration;
using System.Data.SqlClient;
using Microsoft.SharePoint.ApplicationPages;
using System.Linq;
using System.IO;
using Pulsus.Targets;
using Pulsus.Configuration;

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
			Guid currentApplicationId = new Guid(Selector.CurrentId);
			var application = SPWebService.ContentService.WebApplications.Where(a => a.Id.Equals(currentApplicationId)).FirstOrDefault();
			if (application != null)
			{
				string pulsusConfigPath = Path.Combine(application.GetIisSettingsWithFallback(SPUrlZone.Default).Path.FullName, "Pulsus.config");
				var emailTarget = PulsusConfiguration.Load(pulsusConfigPath).Targets.Where(t => t.Value is EmailTarget).Select(t => t.Value).FirstOrDefault();
				if (emailTarget != null)
				{
					emailTargetFromAddress.Text = ((EmailTarget)emailTarget).From;
					emailTargetToAddress.Text = ((EmailTarget)emailTarget).To;
					emailTargetServer.Text = ((EmailTarget)emailTarget).SmtpServer;
					emailTargetServerPort.Text = ((EmailTarget)emailTarget).SmtpPort.ToString();
				}
			}
		}

		protected void okButton_Click(object sender, EventArgs e)
		{
			Guid currentApplicationId = new Guid(Selector.CurrentId);
			var application = SPWebService.ContentService.WebApplications.Where(a => a.Id.Equals(currentApplicationId)).FirstOrDefault();
			if (application != null)
			{
				string pulsusConfigDefaultContent =
					@"<pulsus logKey=""" + application.Name + @""" debug=""false"">
	<targets>
		<target name=""email"" type=""EmailTarget"" from=""" + emailTargetFromAddress.Text + @""" to=""" + emailTargetToAddress.Text + @""" smtpServer=""" + emailTargetServer.Text + @""" smtpPort=""" + emailTargetServerPort.Text + @""" />
	</targets>
</pulsus>";
				Enum.GetValues(typeof(SPUrlZone)).Cast<SPUrlZone>().ToList().ForEach(z =>
				{
					string pulsusConfigPath = Path.Combine(application.GetIisSettingsWithFallback(z).Path.FullName, "Pulsus.config");
					File.WriteAllText(pulsusConfigPath, pulsusConfigDefaultContent);
				});
				
				Guid pulsusFeatureId = new Guid("7465ed3a-f5cc-46ba-8205-0ba5fc0e8b35");
				if (!application.Features.Any(f => f.DefinitionId.Equals(pulsusFeatureId)))
					application.Features.Add(pulsusFeatureId);
			}
			base.EndOperation(0);
		}
	}
}
