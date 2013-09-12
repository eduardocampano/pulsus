using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Pulsus.Internal
{
    internal static class TypeHelpers
    {
        public static void LoadDefaultValues(object instance)
        {
            var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var defaultValue = (DefaultValueAttribute)property.GetCustomAttributes(typeof(DefaultValueAttribute), true).FirstOrDefault();
                if (defaultValue != null)
                    property.SetValue(instance, defaultValue.Value, null);
            }
        }

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
