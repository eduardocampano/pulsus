using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus.Configuration
{
    public class PulsusXmlConfiguration : PulsusConfiguration
    {
        private static IDictionary<string, Type> KnownTargetTypes = GetKnownTargetTypes();
        private FileSystemWatcher _fileWatcher;
        private string _fileName;
        private bool _disposing;

        public PulsusXmlConfiguration(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            if (fileName.IsNullOrEmpty())
                throw new ArgumentException("The fileName is not valid", "fileName");

            _fileName = fileName;

            Initialize();
        }

        public PulsusXmlConfiguration(XElement pulsusElement)
        {
            if (pulsusElement == null)
                throw new ArgumentNullException("pulsusElement");

            Initialize(pulsusElement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopFileWatching();
            }

            base.Dispose(disposing);
        }

        protected override void Initialize()
        {
            var xDocument = XDocument.Load(_fileName);
            var pulsusElement = xDocument.Root;
            
            Initialize(pulsusElement);
            
            StartFileWatching();
        }

        protected void Initialize(XElement pulsusElement)
        {
            LoadAttributes(this, pulsusElement);
            AddTargets(pulsusElement);
            base.Initialize();
        }

        protected void AddTargets(XElement xElement)
        {
            var targetsElement = xElement.Element("targets");
            if (targetsElement == null)
                return;

            var targetsToAdd = new List<Target>();
            foreach (var targetElement in targetsElement.Elements("target"))
            {
                var target = ParseTarget(targetElement);
                if (target != null)
                    targetsToAdd.Add(target);
            }

            if (targetsToAdd.Any())
            {
                // TODO: implement read-write lock here
                Targets.Clear();
                foreach (var target in targetsToAdd)
                    AddTarget(target.Name, target);
            }
        }

        protected Target ParseTarget(XElement targetElement)
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

                var wrappedTarget = ParseTarget(wrappedTargetElement);
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

        protected void StartFileWatching()
        {
            if (_fileWatcher != null)
                return;

            PulsusLogger.Write("Watching file '{0}'", _fileName);
            _fileWatcher = new FileSystemWatcher()
            {
                Path = Path.GetDirectoryName(_fileName),
                Filter = Path.GetFileName(_fileName),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.Attributes
            };

            _fileWatcher.Created += OnWatcherChanged;
            _fileWatcher.Changed += OnWatcherChanged;
            _fileWatcher.EnableRaisingEvents = true;
        }

        protected void StopFileWatching()
        {
            if (_fileWatcher != null)
            {
                PulsusLogger.Write("Stopped watching file '{0}'", _fileName);
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }
        }

        protected void OnWatcherChanged(object source, FileSystemEventArgs e)
        {
            PulsusLogger.Write("Watched file '{0}' has changed", _fileName);
            Initialize();
        }
    }
}
