using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace IOSLib
{
    [Serializable]
    public sealed class BuildNumber : ICloneable, IComparable, IComparable<BuildNumber?>, IEquatable<BuildNumber?>
    {
        // AssemblyName depends on the order staying the same
        private readonly int _Major; // Do not rename (binary serialization)
        private readonly char _Minor; // Do not rename (binary serialization)
        private readonly int _Build; // Do not rename (binary serialization)

        public BuildNumber(int major, char minor, int build)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException(nameof(major));

            if (!char.IsUpper(minor))
                throw new ArgumentOutOfRangeException(nameof(minor));

            if (build < 0)
                throw new ArgumentOutOfRangeException(nameof(build));

            _Major = major;
            _Minor = minor;
            _Build = build;
        }

        public BuildNumber(string buildNumber)
        {
            BuildNumber b = Parse(buildNumber);
            _Major = b.Major;
            _Minor = b.Minor;
            _Build = b.Build;
        }

        private BuildNumber(BuildNumber buildNumber)
        {
            Debug.Assert(buildNumber != null);

            _Major = buildNumber._Major;
            _Minor = buildNumber._Minor;
            _Build = buildNumber._Build;
        }

        public int CompareTo(object? buildNumber)
        {
            if (buildNumber == null)
            {
                return 1;
            }

            if (buildNumber is BuildNumber v)
            {
                return CompareTo(v);
            }

            throw new ArgumentException();
        }

        public int CompareTo(BuildNumber? value)
        {
            return
                object.ReferenceEquals(value, this) ? 0 :
                value is null ? 1 :
                _Major != value._Major ? (_Major > value._Major ? 1 : -1) :
                _Minor != value._Minor ? (_Minor > value._Minor ? 1 : -1) :
                _Build != value._Build ? (_Build > value._Build ? 1 : -1) :
                0;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BuildNumber);
        }

        public bool Equals(BuildNumber? obj)
        {
            return object.ReferenceEquals(obj, this) ||
                (!(obj is null) &&
                _Major == obj._Major &&
                _Minor == obj._Minor &&
                _Build == obj._Build);
        }

        public override int GetHashCode()
        {
            // Let's assume that most buildNumber numbers will be pretty small and just
            // OR some lower order bits together.

            int accumulator = 0;

            accumulator |= (_Major & 0x0000000F) << 28;
            accumulator |= (_Minor - 'A' & 0x000000FF) << 20;
            accumulator |= (_Build & 0x000000FF);

            return accumulator;
        }

        public override string ToString() => $"{Major}{Minor}{Build}";

        public object Clone()
        {
            return new BuildNumber(this);
        }

        // Properties for setting and getting buildNumber numbers
        public int Major => _Major;

        public char Minor => _Minor;

        public int Build => _Build;

        public static BuildNumber Parse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return ParseBuildNumber(input, throwOnFailure: true)!;
        }

        public static bool TryParse(string? input, out BuildNumber? result)
        {
            if (input == null)
            {
                result = null;
                return false;
            }

            return (result = ParseBuildNumber(input, throwOnFailure: false)) != null;
        }

        private static BuildNumber? ParseBuildNumber(string input, bool throwOnFailure)
        {
            var m= Regex.Match(input, @"^(\d+)([A-Z])(\d+)$");
            if (!m.Success)
            {
                if (throwOnFailure)
                {
                    throw new ArgumentException();
                }
                return null;
            }
            return new BuildNumber(int.Parse(m.Groups[1].Value), m.Groups[2].Value[0], int.Parse(m.Groups[3].Value));
        }

        // Force inline as the true/false ternary takes it above ALWAYS_INLINE size even though the asm ends up smaller
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(BuildNumber? v1, BuildNumber? v2)
        {
            // Test "right" first to allow branch elimination when inlined for null checks (== null)
            // so it can become a simple test
            if (v2 is null)
            {
                // return true/false not the test result https://github.com/dotnet/runtime/issues/4207
                return (v1 is null) ? true : false;
            }

            // Quick reference equality test prior to calling the virtual Equality
            return ReferenceEquals(v2, v1) ? true : v2.Equals(v1);
        }

        public static bool operator !=(BuildNumber? v1, BuildNumber? v2) => !(v1 == v2);

        public static bool operator <(BuildNumber? v1, BuildNumber? v2)
        {
            if (v1 is null)
            {
                return !(v2 is null);
            }

            return v1.CompareTo(v2) < 0;
        }

        public static bool operator <=(BuildNumber? v1, BuildNumber? v2)
        {
            if (v1 is null)
            {
                return true;
            }

            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >(BuildNumber? v1, BuildNumber? v2) => v2 < v1;

        public static bool operator >=(BuildNumber? v1, BuildNumber? v2) => v2 <= v1;
    }
}
