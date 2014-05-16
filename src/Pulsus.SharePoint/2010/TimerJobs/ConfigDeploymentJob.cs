using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Pulsus.SharePoint.Core;

namespace Pulsus.SharePoint.TimerJobs
{
	public class ConfigDeploymentJob : SPJobDefinition
	{
		public ConfigDeploymentJob()
			: base()
		{
		}

		public ConfigDeploymentJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
			: base(jobName, service, server, targetType)
		{
		}

		public ConfigDeploymentJob(string jobName, SPWebApplication webApplication)
			: base(jobName, webApplication, null, SPJobLockType.None)
		{
		}

		public override void Execute(Guid targetInstanceId)
		{
			var application = this.Parent as SPWebApplication;
			string config = "";
			if (application.Properties.ContainsKey(Constants.PulsusConfigKey))
				config = Convert.ToString(application.Properties[Constants.PulsusConfigKey]);

			if (!String.IsNullOrEmpty(config))
			{
				Enum.GetValues(typeof(SPUrlZone)).Cast<SPUrlZone>().ToList().ForEach(z =>
				{
					var applicationConfig = application.GetPulsusConfigPath(z);
					File.WriteAllText(applicationConfig, config);
				});
			}

			base.Execute(targetInstanceId);
		}
	}
}