using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.PropertyList.Native.Plist;

namespace MobileDeviceSharp.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain an integer value.
    /// </summary>
    public sealed class PlistInteger
#if NET7_0_OR_GREATER
        : PlistNumberNode<long>, INumber<PlistInteger>
#else
        : PlistValueNode<long>
#endif
    {
        /// <summary>
        /// Create <see cref="long"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Uint"/> to wrap.</param>
        public PlistInteger(PlistHandle handle) : base(handle)
        {

        }

        /// <summary>
        /// Create <see cref="long"/> plist node.
        /// </summary>
        public PlistInteger(): this(default(long))
        {

        }

        /// <summary>
        /// Create <see cref="long"/> plist node from a value.
        /// </summary>
        /// <param name="Value">the <see cref="long"/> value.</param>
        public PlistInteger(long Value) : base(plist_new_uint(Value))
        {

        }

        /// <inheritdoc/>.
        public override long Value
        {
            get
            {
                plist_get_uint_val(Handle, out long value);
                return value;
            }
            set => plist_set_uint_val(Handle, value);
        }
#if NET7_0_OR_GREATER
        public static PlistInteger One => new PlistInteger(1);

        static int Radix => 2;

        private static PlistInteger m_zero = new PlistInteger(0);

        private static PlistInteger m_one = new PlistInteger(1);

        static PlistInteger Zero ;

        static IAdditiveIdentity<PlistInteger,PlistInteger> AdditiveIdentity => m_zero;

        static IMultiplicativeIdentity<PlistInteger, PlistInteger> MultiplicativeIdentity => m_one;

        static PlistInteger INumberBase<PlistInteger>.One => throw new NotImplementedException();

        static int INumberBase<PlistInteger>.Radix => throw new NotImplementedException();

        static PlistInteger INumberBase<PlistInteger>.Zero => throw new NotImplementedException();

        static PlistInteger IAdditiveIdentity<PlistInteger, PlistInteger>.AdditiveIdentity => throw new NotImplementedException();

        static PlistInteger IMultiplicativeIdentity<PlistInteger, PlistInteger>.MultiplicativeIdentity => throw new NotImplementedException();

        int IComparable.CompareTo(object? obj) => throw new NotImplementedException();

        public int CompareTo(PlistInteger? other)
        {
            return plist_uint_val_compare(Handle, unchecked((uint)Value));
        }
        public static PlistInteger Abs(PlistInteger value) => new PlistInteger(Math.Abs(value.Value));
        static bool INumberBase<PlistInteger>.IsCanonical(PlistInteger value) => true;
        static bool INumberBase<PlistInteger>.IsComplexNumber(PlistInteger value) => false;
        public static bool IsEvenInteger(PlistInteger value) => long.IsEvenInteger(value.Value);
        static bool INumberBase<PlistInteger>.IsFinite(PlistInteger value) => true;
        static bool INumberBase<PlistInteger>.IsImaginaryNumber(PlistInteger value) => false;
        static bool INumberBase<PlistInteger>.IsInfinity(PlistInteger value) => false;
        static bool INumberBase<PlistInteger>.IsInteger(PlistInteger value) => true;
        static bool INumberBase<PlistInteger>.IsNaN(PlistInteger value) => false;
        public static bool IsNegative(PlistInteger value) => long.IsNegative(value.Value);
        static bool INumberBase<PlistInteger>.IsNegativeInfinity(PlistInteger value) => false;
        static bool INumberBase<PlistInteger>.IsNormal(PlistInteger value) => true;
        public static bool IsOddInteger(PlistInteger value) => long.IsOddInteger(value.Value);
        public static bool IsPositive(PlistInteger value) => long.IsPositive(value.Value);
        static bool INumberBase<PlistInteger>.IsPositiveInfinity(PlistInteger value) => false;
        static bool INumberBase<PlistInteger>.IsRealNumber(PlistInteger value) => true;
        static bool INumberBase<PlistInteger>.IsSubnormal(PlistInteger value) => false;
        static bool INumberBase<PlistInteger>.IsZero(PlistInteger value) => value.Value ==0;
        public static PlistInteger MaxMagnitude(PlistInteger x, PlistInteger y)
        {
            var xval = x.Value;
            var yval = y.Value;
            var result = long.MaxMagnitude(xval, yval);
            return new PlistInteger(result);
        }
        static PlistInteger INumberBase<PlistInteger>.MaxMagnitudeNumber(PlistInteger x, PlistInteger y) => MaxMagnitude(x, y);
        public static PlistInteger MinMagnitude(PlistInteger x, PlistInteger y)
        {
            var xval = x.Value;
            var yval = y.Value;
            var result = long.MaxMagnitude(xval, yval);
            return new PlistInteger(result);
        }
        static PlistInteger INumberBase<PlistInteger>.MinMagnitudeNumber(PlistInteger x, PlistInteger y) => MinMagnitude(x, y);
        public static PlistInteger Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => new PlistInteger(long.Parse(s, style, provider);
        public static PlistInteger Parse(string s, NumberStyles style, IFormatProvider? provider) => new PlistInteger(long.Parse(s, style, provider);
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out PlistInteger result)
        {
            
            var success = long.TryParse(s, style, provider,out long baseResult);
            result = success ? new PlistInteger(baseResult) : null;
            return success;
        }

        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out PlistInteger result)
        {
            var success = long.TryParse(s, style, provider, out long baseResult);
            result = success ? new PlistInteger(baseResult) : null;
            return success;
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider) => Value.TryFormat(destination, out charsWritten, format, provider);
        public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format);
        public static PlistInteger Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            return new PlistInteger(long.Parse(s, provider));
        }
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out PlistInteger result)
        {
            var success = long.TryParse(s, provider, out long baseResult);
            result = success ? new PlistInteger(baseResult) : null;
            return success;
        }
        public static PlistInteger Parse(string s, IFormatProvider? provider)
        {
            return new PlistInteger(long.Parse(s, provider));
        }
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out PlistInteger result)
        {
            var success = long.TryParse(s, provider, out long baseResult);
            result = success ? new PlistInteger(baseResult) : null;
            return success;
        }

        public static bool operator >(PlistInteger left, PlistInteger right)
        {
            return left.Value > right.Value;
        }

        public static bool operator >=(PlistInteger left, PlistInteger right)
        {
            return left.Value >= right.Value;
        }

        public static bool operator <(PlistInteger left, PlistInteger right)
        {
            return left.Value < right.Value;
        }
        public static bool operator <=(PlistInteger left, PlistInteger right)
        {
                return left.Value <= right.Value;
        }
        public static PlistInteger operator %(PlistInteger left, PlistInteger right)
        {
            return left % right;
        }
        public static PlistInteger operator +(PlistInteger left, PlistInteger right)
        {
            return new PlistInteger(left.Value + right.Value);
        }

        public static PlistInteger operator --(PlistInteger value)
        {
            return new PlistInteger(value.Value--);
        }
        public static PlistInteger operator /(PlistInteger left, PlistInteger right)
        {
            return new PlistInteger(left.Value / right.Value);
        }
        public static bool operator ==(PlistInteger? left, PlistInteger? right)
        {
            return (left?.Equals(right)).GetValueOrDefault(false);
        }
        public static bool operator !=(PlistInteger? left, PlistInteger? right)
        {
            return !(left?.Equals(right)).GetValueOrDefault(false);
        }
        public static PlistInteger operator ++(PlistInteger value)
        {
            return new PlistInteger(value.Value + 1);
        }
        public static PlistInteger operator *(PlistInteger left, PlistInteger right)
        {
            return new PlistInteger(left.Value * right.Value);
        }
        public static PlistInteger operator -(PlistInteger left, PlistInteger right)
        {
            return new PlistInteger(left.Value - right.Value);
        }

        public static PlistInteger operator -(PlistInteger value)
        {
            return new PlistInteger(-value.Value);
        }

        public static PlistInteger operator +(PlistInteger value)
        {
            return new PlistInteger(+value.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (obj is PlistNode plistNode && Equals(plistNode))
            {
                return true;
            }

            if (obj is PlistInteger integer)
            {
                return Equals(integer);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public bool Equals(PlistInteger? other) => this.Value.Equals(other.Value);
        static bool TryConvertFromChecked<TOther>(TOther value, out PlistInteger result)
        {
            Int64 resultValue
            checked
            {
                try
                {
                    resultValue = Convert.ToInt64(value);
                } catch (OverflowException) {
                    result = null;
                    return false;
                }
                result = new PlistInteger(resultValue);
            }
        }
        static bool INumberBase<PlistInteger>.TryConvertFromSaturating<TOther>(TOther value, out PlistInteger result) => throw new NotImplementedException();
        static bool INumberBase<PlistInteger>.TryConvertFromTruncating<TOther>(TOther value, out PlistInteger result) => throw new NotImplementedException();
        static bool INumberBase<PlistInteger>.TryConvertToChecked<TOther>(PlistInteger value, out TOther result) => throw new NotImplementedException();
        static bool INumberBase<PlistInteger>.TryConvertToSaturating<TOther>(PlistInteger value, out TOther result) => throw new NotImplementedException();
        static bool INumberBase<PlistInteger>.TryConvertToTruncating<TOther>(PlistInteger value, out TOther result) => throw new NotImplementedException();
    }
}
