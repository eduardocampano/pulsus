using System;
using System.Globalization;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Pulsus;
using Pulsus.SharePoint.Core;
using Pulsus.SharePoint.Core.Data;

namespace Pulsus.SharePoint.Layouts
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

            if (Request.QueryString["log"] != null)
            {
                LogManager.EventFactory.Create()
                    .Level(LoggingEventLevel.Debug)
                    .AddData("customKey", "customValue")
                    .Text(Request.QueryString["log"])
                    .Push();
            }
        }

        protected void HandleAjaxRequest()
        {
            var eventId = Request.QueryString["EventId"];
            if (!eventId.IsNullOrEmpty())
                HandleEventDetailsRequest(eventId);
            else
                HandleGridDataRequest();
        }

        protected void HandleEventDetailsRequest(string eventId)
        {
            var repository = new DatabaseLoggingEventRepository();
            var loggingEvent = repository.Get(eventId);
            if (loggingEvent == null)
            {
                Response.HtmlResult("<h1>Not Found</h1>");
                return;
            }

            var loggingEventTemplateModel = new LoggingEventTemplateModel(loggingEvent);
            var loggingEventTemplate = new LoggingEventTemplate(loggingEventTemplateModel);
            var htmlResponse = loggingEventTemplate.TransformText();
            Response.HtmlResult(htmlResponse);
        }

        protected void HandleGridDataRequest()
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
