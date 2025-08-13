using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace OtoBatchEditor.Utility
{
    public class IgnoreReactiveObjectProperties : TypeInspectorSkeleton
    {
        private readonly ITypeInspector _innerInspector;
        private readonly HashSet<string> _reactiveObjectPropertyNames;

        public IgnoreReactiveObjectProperties(ITypeInspector innerInspector)
        {
            _innerInspector = innerInspector ?? throw new ArgumentNullException(nameof(innerInspector));
            _reactiveObjectPropertyNames = new HashSet<string>(GetAllPropertyNames(typeof(ReactiveObject)));
        }

        private static IEnumerable<string> GetAllPropertyNames(Type type)
        {
            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
            {
                foreach (var prop in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    yield return prop.Name;
                }
            }
        }

        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
        {
            foreach (var prop in _innerInspector.GetProperties(type, container))
            {
                if (_reactiveObjectPropertyNames.Contains(prop.Name))
                {
                    // ReactiveObject由来のプロパティはスキップ
                    continue;
                }
                yield return prop;
            }
        }

        public override string GetEnumName(Type enumType, string name)
        {
            return _innerInspector.GetEnumName(enumType, name);
        }

        public override string GetEnumValue(object enumValue)
        {
            return _innerInspector.GetEnumValue(enumValue);
        }
    }
}
