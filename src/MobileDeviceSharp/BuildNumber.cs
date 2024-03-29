﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MobileDeviceSharp
{
    /// <summary>
    /// Represent an OS apple build number
    /// </summary>
    [Serializable]
    public sealed class BuildNumber : ICloneable, IComparable, IComparable<BuildNumber?>, IEquatable<BuildNumber?>
    {
        // AssemblyName depends on the order staying the same
        private readonly int _major; // Do not rename (binary serialization)
        private readonly char _minor; // Do not rename (binary serialization)
        private readonly int _build; // Do not rename (binary serialization)
        private readonly char? _revision; // Do not rename (binary serialization)

        /// <summary>
        /// Create an Apple OS build number.
        /// </summary>
        /// <param name="major">Major build number (generaly increased when the OS major version increased).</param>
        /// <param name="minor">Minor build number (generaly increased when the OS minor version increased).</param>
        /// <param name="build">build build number (generaly increased when the OS minor or build version increased).</param>
        /// <param name="revision">Generaly only present for beta release.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public BuildNumber(int major, char minor, int build, char? revision)
        {
            if (major < 0)
                throw new ArgumentOutOfRangeException(nameof(major));

            if (minor is < 'A' or > 'Z')
                throw new ArgumentOutOfRangeException(nameof(minor));

            if (build < 0)
                throw new ArgumentOutOfRangeException(nameof(build));

            if (revision.HasValue && revision.Value is < 'a' or > 'z')
                throw new ArgumentOutOfRangeException(nameof(revision));

            _major = major;
            _minor = minor;
            _build = build;
            _revision = revision;
        }

        /// <summary>
        /// Create an Apple OS build number.
        /// </summary>
        /// <param name="major">Major build number (generaly increased when the OS major version increased).</param>
        /// <param name="minor">Minor build number (generaly increased when the OS minor version increased).</param>
        /// <param name="build">build build number (generaly increased when the OS build version increased).</param>
        public BuildNumber(int major, char minor, int build) : this(major, minor, build, null)
        {

        }

        /// <summary>
        /// Create an Apple OS build from string
        /// </summary>
        /// <param name="buildNumber">The build number string</param>
        public BuildNumber(string buildNumber)
        {
            BuildNumber b = Parse(buildNumber);
            _major = b.Major;
            _minor = b.Minor;
            _build = b.Build;
            _revision = b._revision;
        }

        private BuildNumber(BuildNumber buildNumber)
        {
            Debug.Assert(buildNumber is not null);

            _major = buildNumber._major;
            _minor = buildNumber._minor;
            _build = buildNumber._build;
            _revision = buildNumber._revision;
        }

        /// <inheritdoc/>
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

            throw new ArgumentException(nameof(buildNumber));
        }

        /// <inheritdoc/>
        public int CompareTo(BuildNumber? value)
        {
            return
                object.ReferenceEquals(value, this) ? 0 :
                value is null ? 1 :
                _major != value._major ? (_major > value._major ? 1 : -1) :
                _minor != value._minor ? (_minor > value._minor ? 1 : -1) :
                _build != value._build ? (_build > value._build ? 1 : -1) :
                _revision != value._revision ? (_revision.GetValueOrDefault('\0') > value._revision.GetValueOrDefault('\0') ? 1 : -1) :
                0;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as BuildNumber);
        }

        /// <inheritdoc/>
        public bool Equals(BuildNumber? obj)
        {
            return object.ReferenceEquals(obj, this) ||
                (obj is not null &&
                _major == obj._major &&
                _minor == obj._minor &&
                _build == obj._build &&
                _revision == obj._revision);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // Let's assume that most buildNumber numbers will be pretty small and just
            // OR some lower order bits together.

            int accumulator = 0;

            accumulator |= (_major & 0x0000000F) << 28;
            accumulator |= (_minor - 'A' & 0x000000FF) << 20;
            accumulator |= (_build & 0x000000FF << 12);
            accumulator |= ((_revision.HasValue ? _revision.Value - 'a' + 1 : 0) & 0x00000FFF);

            return accumulator;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Major}{Minor}{Build}{(_revision.HasValue ? _revision : string.Empty)}";

        /// <inheritdoc/>
        public object Clone()
        {
            return new BuildNumber(this);
        }

        // Properties for setting and getting buildNumber numbers

        /// <summary>
        /// Get the major part of the build number (generaly increased when the OS major version increased.
        /// </summary>
        public int Major => _major;

        /// <summary>
        /// Get the minor part of the build number (generaly increased when the OS minor version increased.
        /// </summary>
        public char Minor => _minor;

        /// <summary>
        /// Get the build part of the build number (generaly increased when the OS minor or build version increased.
        /// </summary>
        public int Build => _build;

        /// <summary>
        /// Get the revision part of the build number (generaly only present for beta releasep.
        /// </summary>
        public char? Revision => _revision;

        /// <summary>
        /// Parse the <paramref name="input"/> <see cref="string"/> to a build number
        /// </summary>
        /// <param name="input">The imput string</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static BuildNumber Parse(string input)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(input);
#else
            if (input == null)
                throw new ArgumentNullException(nameof(input));
#endif
            return ParseBuildNumber(input.AsSpan(), true)!;
        }

        /// <summary>
        /// Parse the <paramref name="input"/> <see cref="string"/> to a build number
        /// </summary>
        /// <param name="input">The imput string</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static BuildNumber Parse(ReadOnlySpan<char> input)
            => ParseBuildNumber(input, throwOnFailure: true)!;

        /// <summary>
        /// Tries to parse the <see cref="string"/> representation of a build number to an equivalent
        /// <see cref="BuildNumber"/> object, and returns a value that indicates whether the conversion
        /// succeeded.
        /// </summary>
        /// <param name="input">A <see cref="string"/> that contains a build number to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="BuildNumber"/> equivalent of the values
        /// that is contained in input, if the conversion succeeded, or <see langword="null"></see>
        /// if the conversion failed.</param>
        /// <returns><see langword="true"/> if s was converted successfully; otherwise, <see langword="false"/>.</returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public static bool TryParse(string? input, [NotNullWhen(true)] out BuildNumber? result)
#else
        public static bool TryParse(string? input, out BuildNumber? result)
#endif
        {
            return (result = ParseBuildNumber(input.AsSpan(), throwOnFailure: false)) is not null;
        }

        /// <summary>
        /// Tries to parse the <see cref="string"/> representation of a build number to an equivalent
        /// <see cref="BuildNumber"/> object, and returns a value that indicates whether the conversion
        /// succeeded.
        /// </summary>
        /// <param name="input">A <see cref="ReadOnlySpan{Char}"/> that contains a build number to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="BuildNumber"/> equivalent of the values
        /// that is contained in input, if the conversion succeeded, or <see langword="null"></see>
        /// if the conversion failed.</param>
        /// <returns><see langword="true"/> if s was converted successfully; otherwise, <see langword="false"/>.</returns>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        public static bool TryParse(ReadOnlySpan<char> input, [NotNullWhen(true)] out BuildNumber? result)
#else
        public static bool TryParse(ReadOnlySpan<char> input, out BuildNumber? result)
#endif
        {
            return (result = ParseBuildNumber(input, throwOnFailure: false)) is not null;
        }

        private static BuildNumber? ParseBuildNumber(ReadOnlySpan<char> input, bool throwOnFailure)
        {
            int index = 0;
            int numberStart = 0;
            int major, build;
            char minor;
            char? revision = null;

            // Extract first number
            while (index < input.Length && char.IsDigit(input[index]))
            {
                index++;
            }
            var majorSpan = input.Slice(numberStart, index - numberStart);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1
            if (numberStart == index || !int.TryParse(majorSpan, out major))
#else
            if (numberStart == index || !int.TryParse(majorSpan.ToString(), out major))
#endif
            {
                if (throwOnFailure) throw new ArgumentException();
                return null;
            }

            // Check if second character is an uppercase letter
            if (index >= input.Length || input[index] is < 'A' or > 'Z')
            {
                if (throwOnFailure) throw new ArgumentException();
                return null;
            }
            minor = input[index];
            index++;

            // Extract second number
            numberStart = index;
            while (index < input.Length && char.IsDigit(input[index]))
            {
                index++;
            }
            var buildSpan = input.Slice(numberStart, index - numberStart);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            if (numberStart == index || !int.TryParse(buildSpan, out build))
#else
            if (numberStart == index || !int.TryParse(buildSpan.ToString(), out build))
#endif
            {
                if (throwOnFailure) throw new ArgumentException();
                return null;
            }

            // Check if fourth character (if present) is a lowercase letter
            if (index < input.Length)
            {
                if (revision is >= 'a' and <= 'z')
                {
                    revision = input[index];
                }
                else
                {
                    if (throwOnFailure) throw new ArgumentException();
                    return null;
                }
            }
            return new BuildNumber(major, minor, build, revision);
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
            return ReferenceEquals(v2, v1) || v2.Equals(v1);
        }

        public static bool operator !=(BuildNumber? v1, BuildNumber? v2) => !(v1 == v2);

        public static bool operator <(BuildNumber? v1, BuildNumber? v2)
        {
            if (v1 is null)
            {
                return v2 is not null;
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
