using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace IOSLib.SourceGenerator
{
    internal abstract class SourceCodeInfoBase 
    {
        internal abstract IReadOnlyDictionary<string, string> BuildSource();

        internal void AddTo(GeneratorExecutionContext context)
        {
            foreach (var item in this.BuildSource())
            {
                context.AddSource(item.Key, item.Value);
            }
        }
    }
}