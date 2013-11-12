using System;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Pulsus.SharePoint.Core;
using Pulsus.SharePoint.Core.Data;

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
        }

        protected void HandleAjaxRequest()
        {
            var to = DateTime.Now;
            var from = to.AddDays(-30);
            var repository = new DatabaseLoggingEventRepository();
            var items = repository.List(from, to, 0, 100);
            Response.JsonResult(items);
        }
    }
}
