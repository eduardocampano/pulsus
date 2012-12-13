namespace Pulsus.Configuration
{
    public class MsSqlSettings : IMsSqlSettings
    {
        public MsSqlSettings()
        {
        }

        internal MsSqlSettings(MsSqlElement msSqlElement)
        {
            Enabled = msSqlElement.Enabled;
            ConnectionName = msSqlElement.ConnectionName;
            DatabaseName = msSqlElement.DatabaseName;
            Schema = msSqlElement.Schema;
            TableName = msSqlElement.TableName;
        }

        public bool Enabled { get; set; }
        public string ConnectionName { get; set; }
        public string DatabaseName { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
    }
}
