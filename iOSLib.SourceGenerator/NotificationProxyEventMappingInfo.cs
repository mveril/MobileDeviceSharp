using IOSLib.SourceGenerator;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace iOSLib.SourceGenerator
{
    class NotificationProxyEventMappingInfo : SourceCodeInfoBase
    {


        internal readonly string className;
        private readonly INamespaceSymbol classNameSpace;
        private readonly INamedTypeSymbol npNameAttrSymbol;
        internal readonly IEnumerable<IEventSymbol> Events;

        internal NotificationProxyEventMappingInfo(INamedTypeSymbol Type, Compilation compilation)
        {
            className = Type.Name;
            classNameSpace = Type.ContainingNamespace;
            npNameAttrSymbol = compilation.GetTypeByMetadataName($"{NotificationProxyEventMappingGenerator.AttrNamespace}.{NotificationProxyEventMappingGenerator.AttrName}");
            Events = Type.GetMembers().Where(s => s.Kind == SymbolKind.Event).Where(e => e.GetAttributes().Any(a => a.AttributeClass.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default))).Cast<IEventSymbol>();
        }
        internal override IReadOnlyDictionary<string, string> BuildSource()
        {
            var source = @"using System;

namespace {0}
{{
    partial class {1}
    {{
        private NotificationProxySession _np;
        private void InitEventWatching(IDevice device)
        {{
            _np = new NotificationProxySession(device);
            _np.NotificationProxyEvent += NotificationProxyHandler;
            _np.ObserveNotification({2});
        }}
        
        private void NotificationProxyHandler(object sender, NotificationProxyEventArgs e)
        {{
            switch(e.EventName)
            {{
{3}
            }};
        }}

{4}

        private void CloseEventWatching()
        {{
            _np.Dispose();
        }}
    }}
}}";
            string GetSwitchCode()
            {
                var sb = new StringBuilder();
                foreach (var item in Events)
                {
                    var npName = item.GetAttributes().First(a => a.AttributeClass.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default)).ConstructorArguments.First().Value;
                    sb.AppendLine($"                case \"{npName}\":");
                    sb.AppendLine($"                    On{item.Name}();");
                    sb.AppendLine($"                    break;");
                }
                sb.AppendLine($"                default:");
                sb.AppendLine($"                    break;");
                return sb.ToString();
            }
            string GetFunctionsCode()
            {
                var funcs = Events.Select(ev =>
                {
                    return $@"        private void On{ev.Name}()
        {{
            {ev.Name}?.Invoke(this, EventArgs.Empty);
        }}";
                });
                return string.Join("\n        \n",funcs);
            }
            string ObserveCode()
            {
                var lists=Events.Select(ev=>$"\"{ev.GetAttributes().First(a => a.AttributeClass.Equals(npNameAttrSymbol, SymbolEqualityComparer.Default)).ConstructorArguments.First().Value}\"");
                return string.Join(", ", lists);
            }
            var kv = new KeyValuePair<string, string>($"{className}.np", string.Format(source, classNameSpace, className, ObserveCode(), GetSwitchCode(), GetFunctionsCode()));
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string> { { kv.Key, kv.Value } });
        }
    }
}
