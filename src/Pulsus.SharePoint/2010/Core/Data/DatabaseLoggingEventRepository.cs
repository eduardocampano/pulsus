using System;
using System.Data;
using System.Linq;
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

        public PageResult<LoggingEventListItem> List(DateTime from, DateTime to, string search, int skip, int take)
        {
            var minSqlDate = new DateTime(1753, 1, 1);

            from = from.Date;
            if (from < minSqlDate)
                from = minSqlDate;

            to = to.Date.AddDays(1);
            if (to < minSqlDate)
                to = minSqlDate;

            var searchWhere = string.IsNullOrEmpty(search) ? string.Empty : "and Text like '%' + @search + '%'";

            var countSql = string.Format(@"select count(1) as Total
                                                 from [LoggingEvents]
                                                 where Date >= @from and Date < @to
                                                 {0}", searchWhere);

            var sql = string.Format(@"select top {0} *
                                      from (
                                        select  ROW_NUMBER() OVER (ORDER BY Date desc) AS RowNum,
                                                EventId,
                                                Level,
                                                Date,
                                                Text,
                                                Tags
                                         from [LoggingEvents]
                                         where Date >= @from and Date < @to
                                         {2}
                                      ) X
                                      where X.RowNum >= {1}", take, skip + 1, searchWhere);

            var parameters = new {from, to, search};

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
