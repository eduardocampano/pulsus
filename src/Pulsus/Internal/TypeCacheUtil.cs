using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;

namespace Pulsus.Internal
{
	// This was adapted from the MVC TypeCacheUtil
	// but without file cache because of the lack of support for cache methods in the BuildManager for .NET 3.5
	internal static class TypeCacheUtil 
	{
		public static IEnumerable<Type> FilterTypesInAssemblies(Predicate<Type> predicate) {
			// Go through all assemblies referenced by the application and search for types matching a predicate
			IEnumerable<Type> typesSoFar = Type.EmptyTypes;

			var assemblies = GetReferencedAssemblies();
			foreach (var assembly in assemblies) {
				Type[] typesInAsm;
				try {
					typesInAsm = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException ex) {
					typesInAsm = ex.Types;
				}
				typesSoFar = typesSoFar.Concat(typesInAsm);
			}
			return typesSoFar.Where(type => TypeIsPublicClass(type) && predicate(type));
		}

		public static Assembly[] GetReferencedAssemblies()
		{
			//If IsHosted, ensure referenced assemblies are loaded prior to returning assemblies loaded in the current AppDomain
			if (HostingEnvironment.IsHosted)
				BuildManager.GetReferencedAssemblies();
			
			return AppDomain.CurrentDomain.GetAssemblies();
		}

		private static bool TypeIsPublicClass(Type type) {
			return (type != null && type.IsPublic && type.IsClass && !type.IsAbstract);
		}
	}
}
