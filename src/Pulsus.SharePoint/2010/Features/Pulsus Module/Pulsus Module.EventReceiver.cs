using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace Pulsus.SharePoint.Features.Pulsus
{
	[Guid("ef7da16a-e11a-4444-a521-cc1fc1e3a78f")]
	public class PulsusFeatureEventReceiver : SPFeatureReceiver
	{
		private static string _owner = "Pulsus";
		private SPWebConfigModification[] _modifications = {
			new SPWebConfigModification() { 
				Name = "add[@name='PulsusErrorLoggingModule']", 
				Owner = _owner, 
				Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, 
				Path = "/configuration/system.web/httpModules", 
				Value = "<add name=\"PulsusErrorLoggingModule\" type=\"" + typeof(ErrorLoggingModule).AssemblyQualifiedName + "\" />"},
			new SPWebConfigModification() { 
				Name = "add[@name='PulsusErrorLoggingModule']", 
				Owner = _owner, 
				Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, 
				Path = "/configuration/system.webServer/modules", 
				Value = "<add name=\"PulsusErrorLoggingModule\" preCondition=\"integratedMode\" type=\"" + typeof(ErrorLoggingModule).AssemblyQualifiedName + "\" /> "},
			new SPWebConfigModification() { 
				Name = "add[@assembly='" + typeof(PulsusFeatureEventReceiver).Assembly.FullName +"']", 
				Owner = _owner, 
				Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, 
				Path = "/configuration/system.web/compilation/assemblies",
				Value = "<add assembly=\"" + typeof(PulsusFeatureEventReceiver).Assembly.FullName + "\" /> "}
		};

		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{
			SPWebApplication application = (SPWebApplication)properties.Feature.Parent;

			foreach (SPWebConfigModification modification in _modifications)
			{
				application.WebConfigModifications.Add(modification);
			}

			application.WebService.ApplyWebConfigModifications();
			application.Update();
		}

		public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		{
			SPWebApplication application = (SPWebApplication)properties.Feature.Parent;

			List<SPWebConfigModification> modificationsToRemove = new List<SPWebConfigModification>();
			foreach (SPWebConfigModification modification in application.WebConfigModifications)
			{
				if (modification.Owner == _owner)
				{
					modificationsToRemove.Add(modification);
				}
			}

			foreach (SPWebConfigModification modification in modificationsToRemove)
			{
				application.WebConfigModifications.Remove(modification);
			}

			application.WebService.ApplyWebConfigModifications();
			application.Update();
		}
	}
}
