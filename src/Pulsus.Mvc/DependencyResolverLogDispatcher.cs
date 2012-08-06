using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Pulsus.Mvc
{
	public class DependencyResolverEventDispatcher : DefaultEventDispatcher
	{
		protected override IEnumerable<ITarget> GetTargets()
		{
			var targets = DependencyResolver.Current.GetServices<ITarget>();

			if (targets != null && targets.Any())
				return targets;

			return base.GetTargets();
		}
	}
}
