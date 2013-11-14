using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Pulsus.SharePoint.Core;

namespace Pulsus.SharePoint.Features.Pulsus
{
    [Guid("4d4badbf-844b-4c21-967f-05737c365c1a")]
    public class PulsusEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //RegisterULSLoggingService(properties);
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //UnRegisterULSLoggingService(properties);
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

            if (service.Status != SPObjectStatus.Online)
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
