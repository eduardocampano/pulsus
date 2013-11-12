using System;
using System.Globalization;
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
            var to = DateTime.Now.Date;
            var from = to.AddMonths(-1);

            var skip = int.Parse(HttpContext.Current.Request.Form["skip"] ?? "0");
            var take = int.Parse(HttpContext.Current.Request.Form["take"] ?? "100");
            var search = HttpContext.Current.Request.Form["search"] ?? string.Empty;
            var periodString = HttpContext.Current.Request.Form["period"];
            if (periodString != null)
            {
                var periodDates = periodString.Split(new[] { " - " }, 2, StringSplitOptions.None);
                if (periodDates.Length == 2)
                {
                    from = DateTime.ParseExact(periodDates[0], "MM/dd/yyyy", CultureInfo.CurrentCulture);
                    to = DateTime.ParseExact(periodDates[1], "MM/dd/yyyy", CultureInfo.CurrentCulture);
                }
            }

            var repository = new DatabaseLoggingEventRepository();
            var result = repository.List(from, to, search, skip, take);
            Response.JsonResult(result);
        }
    }
}
