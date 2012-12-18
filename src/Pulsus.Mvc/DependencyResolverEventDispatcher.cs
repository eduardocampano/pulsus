using System.Linq;
using System.Web.Mvc;
using Pulsus.Targets;

namespace Pulsus.Mvc
{
	public class DependencyResolverEventDispatcher : DefaultEventDispatcher
	{
		protected override Target[] GetTargets()
		{
			var targets = DependencyResolver.Current.GetServices<Target>().ToArray();
			if (targets.Any())
				return targets;

			return base.GetTargets();
		}
	}
}
