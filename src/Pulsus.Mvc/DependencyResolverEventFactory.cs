using System.Web.Mvc;

namespace Pulsus.Mvc
{
	public class DependencyResolverEventFactory : DefaultEventFactory
	{
		public override T Create<T>()
		{
			var instance = DependencyResolver.Current.GetService<T>();

			if (instance == null)
				instance = base.Create<T>();

			return instance;
		}
	}
}
