using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.SharePoint.ApplicationPages.Calendar.Exchange;
using Pulsus.Targets;

namespace Pulsus.SharePoint.Core.Data
{
    internal class DatabaseLoggingEventRepository
    {
        public LoggingEvent Get(string eventId)
        {
            const string sql = @"select * from [LoggingEvents] where EventId = @eventId";
            using (var connection = GetConnection())
            {
                return connection.Query<DatabaseLoggingEvent>(sql, new {eventId})
                                 .Select(x => DatabaseLoggingEvent.Deserialize(x))
                                 .FirstOrDefault();
            }
        }

        public PageResult<LoggingEventListItem> List(DateTime from, DateTime to, LoggingEventLevel? minLevel, LoggingEventLevel? maxLevel, string[] tags, string search, int skip, int take)
        {
            var minSqlDate = new DateTime(1753, 1, 1);

            from = from.Date;
            if (from < minSqlDate)
                from = minSqlDate;

            to = to.Date.AddDays(1);
            if (to < minSqlDate)
                to = minSqlDate;

            var additionalConditions = new List<string>();
            if (!string.IsNullOrEmpty(search)) 
                additionalConditions.Add("Text like '%' + @search + '%'");
            if (minLevel.HasValue)
                additionalConditions.Add("Level >= @minLevel");
            if (maxLevel.HasValue)
                additionalConditions.Add("Level <= @maxLevel");
            if (tags != null && tags.Length > 0)
            {
                foreach (var tag in tags)
                    additionalConditions.Add(string.Format("charindex('{0}', Tags) > 0", tag));
            }

            var conditionsSql = string.Empty;
            if (additionalConditions.Any())
                conditionsSql = "\r\n and " + string.Join("\r\n and ", additionalConditions.ToArray());

            var countSql = string.Format(@"select count(1) as Total
                                                 from [LoggingEvents]
                                                 where Date >= @from and Date < @to {0}", conditionsSql);

            var sql = string.Format(@"select top {0} *
                                      from (
                                        select  ROW_NUMBER() OVER (ORDER BY Date desc) AS RowNum,
                                                EventId,
                                                Level,
                                                Date,
                                                Text,
                                                Tags
                                         from [LoggingEvents]
                                         where Date >= @from and Date < @to {2}
                                      ) X
                                      where X.RowNum >= {1}", take, skip + 1, conditionsSql);
            
            var parameters = new { from, to, search, minLevel = (int?)minLevel, maxLevel = (int?)maxLevel, tags };

            using (var connection = GetConnection())
            {
                var countResult = connection.Query<CountResult>(countSql, parameters).FirstOrDefault();
                var total = countResult == null ? 0 : countResult.Total;
                var page = connection.Query<LoggingEventListItem>(sql, parameters);

                return new PageResult<LoggingEventListItem>(page, total);
            }
        }

        protected IDbConnection GetConnection()
        {
            // search for a database target
            var databaseTargetType = typeof (DatabaseTarget);
            var target = LogManager.Configuration.Targets.Values.FirstOrDefault(x => databaseTargetType.IsAssignableFrom(x.GetType())) as DatabaseTarget;
            
            if (target == null)
                throw new Exception("Unabled to find a DatabaseTarget to query Pulsus logging events");

            return target.GetConnection();
        }
    }
}
