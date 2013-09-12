namespace Pulsus
{
    public interface IFilter
    {
        LoggingEventLevel MinLevel { get; }
        LoggingEventLevel MaxLevel { get; }
        string LogKeyContains { get; }
        string LogKeyStartsWith { get; }
        string TextContains { get; }
        string TextStartsWith { get; }
        string TagsContains { get; }
        double? MinValue { get; }
        double? MaxValue { get; }
    }
}
