namespace Pulsus.Targets
{
	public interface ITarget
	{
		bool Enabled { get; }
		void Push(LoggingEvent[] loggingEvents);
	}
}
