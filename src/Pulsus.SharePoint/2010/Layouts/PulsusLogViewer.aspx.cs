using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Web;
using Microsoft.BusinessData.Infrastructure.SecureStore;
using Microsoft.Office.SecureStoreService.Server;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Pulsus;
using Pulsus.SharePoint.Core;
using Pulsus.SharePoint.Core.Data;
using Pulsus.SharePoint.Targets;

namespace UTC.com.Layouts
{
    public partial class PulsusLogViewer : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SPContext.Current.Web.CurrentUser.IsSiteAdmin)
                SPUtility.HandleAccessDenied(new UnauthorizedAccessException("User must be site collection administrator to view this page."));
        
            if (HttpContext.Current.Request.IsAjaxRequest())
                HandleAjaxRequest();

            if (Request.QueryString["throw"] != null)
                throw new ApplicationException("This is a test exception, please ignore");

            LoadSettings();
        }

        protected void HandleAjaxRequest()
        {
            var to = DateTime.Now;
            var from = to.AddDays(-30);
            var repository = new DatabaseLoggingEventRepository();
            var items = repository.List(from, to, 0, 100);
            Response.JsonResult(items);
        }

        protected void LoadSettings()
        {
            var sb = new StringBuilder();
            foreach (var target in LogManager.Configuration.Targets.Values)
            {
                sb.Append("Target " + target.Name);
                var ulsTarget = target as ULSTarget;
                if (ulsTarget != null)
                {
                    sb.Append("|WriteTrace:" + ulsTarget.WriteTrace);
                    sb.Append("|WriteEvent:" + ulsTarget.WriteEvent);
                }

                var ssTarget = target as SecureStoreDatabaseTarget;
                if (ssTarget != null)
                {
                    sb.Append("|AppId:" + ssTarget.AppId);
                }
                sb.Append("<br /><br />");
            }

            
            error.Text = sb.ToString();
        }

    }
}
