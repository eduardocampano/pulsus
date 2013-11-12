using System;

namespace Pulsus.SharePoint.Core.Data
{
    internal class LoggingEventListItem
    {
        public string EventId { get; set; }
        public LoggingEventLevel Level { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }

        public string LevelString
        {
            get
            {
                return Level.ToString();
            }
        }
    }
}
