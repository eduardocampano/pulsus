using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus.Configuration
{
    public class PulsusXmlConfiguration : PulsusConfiguration
    {
        private static IDictionary<string, Type> KnownTargetTypes = GetKnownTargetTypes();

        public PulsusXmlConfiguration(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (fileName.IsNullOrEmpty())
                throw new ArgumentException("The fileName is not valid", "fileName");

            var xDocument = XDocument.Load(fileName);
            var pulsusElement = xDocument.Root;
            
            Initialize(pulsusElement);
        }

        public PulsusXmlConfiguration(XElement pulsusElement)
        {
            Initialize(pulsusElement);
        }

        protected void Initialize(XElement pulsusElement)
        {
            LoadAttributes(this, pulsusElement);
            Targets = GetTargets(pulsusElement);
        }

        protected IDictionary<string, Target> GetTargets(XElement xElement)
        {
            var targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);

            var targetsElement = xElement.Element("targets");
            if (targetsElement == null)
                return targets;

            foreach (var targetElement in targetsElement.Elements("target"))
            {
                var target = GetTarget(targetElement);
                if (target != null)
                    targets.Add(target.Name, target);
            }

            return targets;
        }

        protected Target GetTarget(XElement targetElement)
        {
            if (targetElement == null)
                return null;

            var name = GetAttributeValue(targetElement, "name");
            var typeName = GetAttributeValue(targetElement, "type");

            if (typeName == null)
                typeName = name.EndsWith("target", StringComparison.OrdinalIgnoreCase) ? name : name + "Target";

            var targetType = FindTargetType(typeName);
            if (targetType == null)
                return null;

            if (typeof(WrapperTarget).IsAssignableFrom(targetType))
            {
                var wrappedTargetElement = targetElement.Element("target");
                if (wrappedTargetElement == null)
                    throw new Exception("No child target element for wrapper target");

                var wrappedTarget = GetTarget(wrappedTargetElement);
                return CreateTarget(targetType, targetElement, wrappedTarget);
            }

            return CreateTarget(targetType, targetElement);
        }

        protected Target CreateTarget(Type type, XElement targetElement = null, Target wrappedTarget = null)
        {
            var target = wrappedTarget == null ? (Target)Activator.CreateInstance(type) : (Target)Activator.CreateInstance(type, wrappedTarget);
            TypeHelpers.LoadDefaultValues(target);
            LoadAttributes(target, targetElement);
            LoadIgnores(target, targetElement);
            return target;
        }

        protected void LoadIgnores(Target target, XElement targetElement)
        {
            var ignoresElement = targetElement.Element("ignores");
            if (ignoresElement == null)
                return;

            foreach (var ignoreElement in ignoresElement.Elements("ignore"))
            {
                var ignore = GetIgnore(ignoreElement);
                if (ignore != null)
                    target.Ignores.Add(ignore);
            }
        }

        protected Ignore GetIgnore(XElement ignoreElement)
        {
            var ignore = new Ignore();
            LoadAttributes(ignore, ignoreElement);
            return ignore;
        }

        protected void LoadAttributes(object instance, XElement targetElement)
        {
            foreach (var xAttribute in targetElement.Attributes())
            {
                var property = instance.GetType().GetProperty(xAttribute.Name.LocalName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null || !property.CanWrite)
                    continue;

                try
                {
                    if (property.PropertyType == typeof(LoggingEventLevel))
                    {
                        try
                        {
                            var enumValue = Enum.Parse(typeof(LoggingEventLevel), xAttribute.Value);
                            property.SetValue(instance, enumValue, null);
                        }
                        catch (Exception)
                        {
                        }

                        continue;
                    }

                    var value = Convert.ChangeType(xAttribute.Value, property.PropertyType);
                    property.SetValue(instance, value, null);
                }
                catch (Exception)
                {
                }
            }
        }

        protected string GetAttributeValue(XElement xElement, string name)
        {
            if (xElement == null)
                return null;

            var attribute = xElement.Attribute(name);

            if (attribute == null)
                return null;

            return attribute.Value;
        }

        protected Type FindTargetType(string name)
        {
            Type targetType;
            if (KnownTargetTypes.TryGetValue(name, out targetType))
                return targetType;

            return null;
        }

        protected static IDictionary<string, Type> GetKnownTargetTypes()
        {
            var targetType = typeof(Target);
            var dictionary = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            var targetTypes = TypeHelpers.GetFilteredTypesFromAssemblies(targetType.IsAssignableFrom);
            foreach (var type in targetTypes)
            {
                dictionary.Add(type.Name, type);
            }

            return dictionary;
        }

        private static Target WrapWithWrapperAsyncTarget(Target target)
        {
            var asyncTargetWrapper = new AsyncWrapperTarget(target);
            PulsusLogger.Write("Wrapped target '{0}' with AsyncWrapperTarget", asyncTargetWrapper.Name);
            target = asyncTargetWrapper;
            return target;
        }
    }
}
