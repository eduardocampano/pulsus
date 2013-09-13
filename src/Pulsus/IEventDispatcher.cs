namespace Pulsus
{
	public interface IEventDispatcher
	{
		void Push(LoggingEvent[] loggingEvents);
	}
}
