using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;

namespace Pulsus.SharePoint.Core
{
    [Guid("D64DEDE4-3D1D-42CC-AF40-DB19F0DFB309")]
    public class ULSLoggingService : SPDiagnosticsServiceBase
    {
        public static class Categories
        {
            public static string Default = "Default";
        }

        public static ULSLoggingService Local
        {
            get { return SPFarm.Local.Services.GetValue<ULSLoggingService>(DefaultName); }
        }

        public static string DefaultName
        {
            get { return "Pulsus Logging Service"; }
        }

        public static string AreaName
        {
            get { return "Pulsus"; }
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            yield return new SPDiagnosticsArea(AreaName, 0, 0, false, new List<SPDiagnosticsCategory>
            {
                new SPDiagnosticsCategory(Categories.Default, null, TraceSeverity.Medium, EventSeverity.Information, 0, 0, false, true)
            });
        }

        public static void WriteTrace(TraceSeverity traceSeverity, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var service = Local;
            if (service == null)
                return;

            try
            {
                var category = service.Areas[AreaName].Categories[Categories.Default];
                service.WriteTrace(1, category, traceSeverity, message);
            }
            catch { }
        }

        public static void WriteEvent(EventSeverity eventSeverity, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            var service = Local;

            if (service == null)
                return;

            try
            {
                var category = service.Areas[AreaName].Categories[Categories.Default];
                service.WriteEvent(1, category, eventSeverity, message);
            }
            catch { }
        }
    }
}
