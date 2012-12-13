namespace Pulsus.Configuration
{
    public interface IMsSqlSettings
    {
        bool Enabled { get; set; }
        string ConnectionName { get; set; }
        string DatabaseName { get; set; }
        string Schema { get; set; }
        string TableName { get; set; }
    }
}