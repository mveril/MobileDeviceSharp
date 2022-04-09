namespace IOSLib.SourceGenerator
{
    internal class NonFreeableHandleInfo :HandleInfoBase
    {

        public NonFreeableHandleInfo(string fullClass) : base(fullClass)
        {

        }

        protected override string GetFreeCode() => $"return {DefaultReturn};";
    }
}
