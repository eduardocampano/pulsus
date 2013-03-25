using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus.Configuration
{
	public class PulsusConfiguration
	{
		private static IDictionary<string, Type> KnownTargetTypes = GetKnownTargetTypes(); 

        public PulsusConfiguration()
        {
			DefaultEventLevel = LoggingEventLevel.Information;
			Enabled = true;
			Debug = false;
			DebugVerbose = false;
			LogKey = "Default";
			Targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);
            ExceptionsToIgnore = new Dictionary<string, Predicate<Exception>>(StringComparer.OrdinalIgnoreCase);
        }

		public static PulsusConfiguration Default
		{
			get
			{
				return GetConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pulsus.config"));
			}
		}

		public static PulsusConfiguration Load(string fileName)
		{
			return GetConfiguration(fileName);
		}

		public bool Enabled { get; set; }
		public bool Debug { get; set; }
		public string DebugFile { get; set; }
		public bool DebugVerbose { get; set; }
		public string LogKey { get; set; }
		public string Tags { get; set; }
		public bool ThrowExceptions { get; set; }
		public bool IncludeHttpContext { get; set; }
        public bool IncludeStackTrace { get; set; }
        public bool IgnoreNotFound { get; set; }

	    public IDictionary<string, Predicate<Exception>> ExceptionsToIgnore { get; private set; }
        public LoggingEventLevel DefaultEventLevel { get; set; }
		public IDictionary<string, Target> Targets { get; private set; } 

		private static PulsusConfiguration GetConfiguration(string fileName)
		{
			var configuration = new PulsusConfiguration();

            if (HostingEnvironment.IsHosted)
                configuration.LogKey = HostingEnvironment.SiteName;

		    if (File.Exists(fileName))
			{
				var xDocument = XDocument.Load(fileName);
				var root = xDocument.Root;
				LoadAttributes(configuration, root);
				configuration.Targets = GetTargets(root);
			}

			if (!configuration.Targets.Any())
			{
				var databaseTarget = new DatabaseTarget();
				LoadDefaultValues(databaseTarget);
				var wrapperTarget = new WrapperTarget(databaseTarget);
				LoadDefaultValues(wrapperTarget);

				configuration.Targets.Add("database", wrapperTarget);
			}

            if (configuration.IgnoreNotFound)
                configuration.ExceptionsToIgnore.Add("notfound", IsNotFoundException);

			SetupDebugging(configuration);

		    return configuration;
		}

		private static void SetupDebugging(PulsusConfiguration configuration)
		{
			if (configuration.Debug)
			{
				if (HostingEnvironment.IsHosted)
				{
					if (string.IsNullOrEmpty(configuration.DebugFile))
						configuration.DebugFile = "~/App_Data/pulsus_log.txt";

					configuration.DebugFile = HostingEnvironment.MapPath(configuration.DebugFile);
				}
				else
				{
					if (string.IsNullOrEmpty(configuration.DebugFile))
						configuration.DebugFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pulsus_log.txt");
				}
			}
		}

		private static IDictionary<string, Target> GetTargets(XElement xElement)
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

		private static Target GetTarget(XElement targetElement)
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

		private static Target CreateTarget(Type type, XElement targetAttributes = null, Target wrappedTarget = null)
		{
			var target = wrappedTarget == null ? (Target)Activator.CreateInstance(type) : (Target)Activator.CreateInstance(type, wrappedTarget);
			LoadDefaultValues(target);
			LoadAttributes(target, targetAttributes);
			return target;
		}

		private static void LoadDefaultValues(object instance)
		{
			var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var defaultValue = (DefaultValueAttribute)property.GetCustomAttributes(typeof(DefaultValueAttribute), true).FirstOrDefault();			
				if (defaultValue != null)
					property.SetValue(instance, defaultValue.Value, null);
			}
		}

		private static void LoadAttributes(object instance, XElement targetElement)
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
                            var enumValue = Enum.Parse(typeof (LoggingEventLevel), xAttribute.Value);
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

		private static string GetAttributeValue(XElement xElement, string name)
		{
			if (xElement == null)
				return null;

			var attribute = xElement.Attribute(name);

			if (attribute == null)
				return null;

			return attribute.Value;
		}

		private static Type FindTargetType(string name)
		{
			Type targetType;
			if (KnownTargetTypes.TryGetValue(name, out targetType))
				return targetType;

			return null;
		}

		private static IDictionary<string, Type> GetKnownTargetTypes()
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

        private static bool IsNotFoundException(Exception ex)
        {
            var httpException = ex as HttpException;
            return httpException != null && httpException.GetHttpCode() == 404;
        }
	}
}
