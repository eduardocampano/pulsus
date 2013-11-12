using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Pulsus.SharePoint.Core;

namespace Pulsus.SharePoint.Features.Pulsus
{
    [Guid("ef7da16a-e11a-4444-a521-cc1fc1e3a78f")]
    public class PulsusFeatureEventReceiver : SPFeatureReceiver
    {
        private const string Owner = "Pulsus";

        private static readonly SPWebConfigModification[] WebConfigModifications = 
        {
            new SPWebConfigModification() 
            { 
                Name = "add[@name='PulsusErrorLoggingModule']", 
                Owner = Owner, 
                Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, 
                Path = "/configuration/system.web/httpModules", 
                Value = "<add name=\"PulsusErrorLoggingModule\" type=\"" + typeof(ErrorLoggingModule).AssemblyQualifiedName + "\" />"
            },
            new SPWebConfigModification() { 
                Name = "add[@name='PulsusErrorLoggingModule']", 
                Owner = Owner, 
                Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, 
                Path = "/configuration/system.webServer/modules", 
                Value = "<add name=\"PulsusErrorLoggingModule\" preCondition=\"integratedMode\" type=\"" + typeof(ErrorLoggingModule).AssemblyQualifiedName + "\" />"
            }
        };

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            AddWebConfigModifications(properties);
            RegisterULSLoggingService(properties);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            RemoveWebConfigModifications(properties);
            UnRegisterULSLoggingService(properties);
        }

        private static void AddWebConfigModifications(SPFeatureReceiverProperties properties)
        {
            var application = (SPWebApplication)properties.Feature.Parent;

            foreach (var modification in WebConfigModifications)
                application.WebConfigModifications.Add(modification);

            application.WebService.ApplyWebConfigModifications();
            application.Update();
        }

        private static void RemoveWebConfigModifications(SPFeatureReceiverProperties properties)
        {
            var application = (SPWebApplication)properties.Feature.Parent;

            var modificationsToRemove = new List<SPWebConfigModification>();
            foreach (var modification in application.WebConfigModifications)
            {
                if (modification.Owner == Owner)
                    modificationsToRemove.Add(modification);
            }

            foreach (var modification in modificationsToRemove)
                application.WebConfigModifications.Remove(modification);

            application.WebService.ApplyWebConfigModifications();
            application.Update();
        }

        public static void RegisterULSLoggingService(SPFeatureReceiverProperties properties)
        {
            var farm = properties.Definition.Farm;

            if (farm == null)
                return;

            var service = ULSLoggingService.Local;

            if (service != null)
                return;

            service = new ULSLoggingService();
            service.Update();

            if(service.Status != SPObjectStatus.Online)
                service.Provision();
        }

        private static void UnRegisterULSLoggingService(SPFeatureReceiverProperties properties)
        {
            var farm = properties.Definition.Farm;

            if (farm == null)
                return;

            var service = ULSLoggingService.Local;

            if (service != null)
                service.Delete();
        }
    }
}
