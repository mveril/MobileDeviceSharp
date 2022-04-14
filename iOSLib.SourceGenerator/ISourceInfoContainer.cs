using System.Collections.Generic;

namespace iOSLib.SourceGenerator
{
    internal interface ISourceInfoContainer
    {
        public IEnumerable<SourceCodeInfoBase> SourceCodeInfos { get; }
    }
}
