using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pulsus.Internal
{
	public static class TypeHelpers
	{
		public static IEnumerable<Assembly> GetAssemblies()
		{
			return TypeCacheUtil.GetReferencedAssemblies();
		}

		public static IEnumerable<Assembly> GetNonSystemAssemblies()
		{
			return TypeCacheUtil.GetReferencedAssemblies()
								.Where(a => !IsSystemAssembly(a));
		}

		public static IEnumerable<Type> GetFilteredTypesFromAssemblies(Predicate<Type> where)
		{
			return TypeCacheUtil.FilterTypesInAssemblies(where);
		}

		private static bool IsSystemAssembly(Assembly assembly)
		{
			var name = assembly.FullName;
			if (name.StartsWith("System") || name.StartsWith("Microsoft") || name.StartsWith("mscorlib"))
				return true;
			return false;
		}
	}
}
