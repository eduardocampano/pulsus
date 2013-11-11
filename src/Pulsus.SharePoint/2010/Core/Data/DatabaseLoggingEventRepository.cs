using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Pulsus.Targets;

namespace Pulsus.SharePoint.Core.Data
{
    internal class DatabaseLoggingEventRepository
    {
        public IEnumerable<LoggingEventListItem> List(DateTime from, DateTime to, int skip, int take)
        {
            var minSqlDate = new DateTime(1753, 1, 1);

            from = from.Date;
            if (from < minSqlDate)
                from = minSqlDate;

            to = to.Date.AddDays(1);
            if (to < minSqlDate)
                to = minSqlDate;

            const string sql = @"select top 100
                                        EventId,
                                        Date,
                                        Text,
                                        Tags
                                 from [LoggingEvents]
                                 where Date >= @from and Date < @to";

            using (var connection = GetConnection())
            {
                return connection.Query<LoggingEventListItem>(sql, new {from, to, skip, take});
            }
        }

        protected IDbConnection GetConnection()
        {
            // search for a database target
            var databaseTargetType = typeof (DatabaseTarget);
            var target = LogManager.Configuration.Targets.Values.FirstOrDefault(x => x.GetType().IsAssignableFrom(databaseTargetType)) as DatabaseTarget;
            
            if (target == null)
                throw new Exception("Unabled to find a DatabaseTarget to query Pulsus logging events");

            return target.GetConnection();
        }
    }
}
