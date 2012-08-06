namespace Pulsus.Repositories
{
	public interface ILoggingEventRepository
	{
		void Initialize();
		void Save(LoggingEvent loggingEvent);
	}
}
