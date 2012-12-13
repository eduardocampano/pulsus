namespace Pulsus
{
	public interface IEventFactory
	{
		LoggingEventBuilder Create();
		T Create<T>() where T : class, ILoggingEventBuilder<T>, new();
	}
}
