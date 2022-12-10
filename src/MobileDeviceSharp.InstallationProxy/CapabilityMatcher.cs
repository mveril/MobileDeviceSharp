using System.Linq;

namespace MobileDeviceSharp.InstallationProxy
{
    public class CapabilityMatcher
    {
#if NET5_0_OR_GREATER
        internal CapabilityMatcher(bool match, IReadOnlySet<string> capabilities, IReadOnlySet<string> mismatchCapabilities)
        {
            Match = match;
            _MatchedCapabilities = new HashSet<string>(capabilities);
            _MismatchCapabilities = mismatchCapabilities.ToHashSet();
            _MatchedCapabilities.ExceptWith(mismatchCapabilities);
        }
#else
        internal CapabilityMatcher(bool match, IEnumerable<string> requestedCapabilities, IEnumerable<string> mismatchCapabilities)
        {
            Match = match;
#if NETSTANDARD2_1_OR_GREATER
            _MismatchCapabilities = mismatchCapabilities.ToHashSet();
            _MatchedCapabilities = requestedCapabilities.ToHashSet();
            RequestedCapabilities = requestedCapabilities.ToHashSet();
#else
            _MismatchCapabilities = new HashSet<string>(mismatchCapabilities);
            _MatchedCapabilities =  new HashSet<string>(requestedCapabilities);
            RequestedCapabilities = new HashSet<string>(requestedCapabilities);
#endif
            _MatchedCapabilities.ExceptWith(mismatchCapabilities);
        }
#endif

        public bool Match { get; }

        public HashSet<string> _MatchedCapabilities;
#if NET5_0_OR_GREATER
        public IReadOnlySet<string> RequestedCapabilities { get; }
#else
        public IReadOnlyCollection<string> RequestedCapabilities { get; }
#endif

#if NET5_0_OR_GREATER
        public IReadOnlySet<string> MatchedCapabilities => _MatchedCapabilities;
#else
        public IReadOnlyCollection<string> MatchedCapabilities => _MatchedCapabilities;
#endif
        public HashSet<string> _MismatchCapabilities;
#if NET5_0_OR_GREATER
        public IReadOnlySet<string> MismatchCapabilities => _MismatchCapabilities;
#else
        public IReadOnlyCollection<string> MismatchCapabilities => _MismatchCapabilities;
#endif
    }
}
