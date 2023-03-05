using MobileDeviceSharp.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Reflection;

namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// Get a dictionarry marshaller to marshal null terminated array of keys followed by values.
    /// </summary>
    /// <typeparam name="T">The type of the key and value.</typeparam>
    /// <typeparam name="TMarshaler">The type of the marshaller of <typeparamref name="T"/>.</typeparam>
    public class DictionaryMarshaler<T,TMarshaler> : CustomMashaler<IReadOnlyDictionary<T,T>> where TMarshaler : CustomMashaler<T>
    {

        private readonly ArrayMarshaller<T,TMarshaler> _arrayMarshaler;

        private static readonly Lazy<DictionaryMarshaler<T,TMarshaler>> s_static_instance = new();

        /// <summary>
        /// Initialize a new <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public DictionaryMarshaler() : this(ArrayMarshaller<T,TMarshaler>.GetInstance())
        {

        }

        /// <summary>
        /// Initialize a new <see cref="Dictionary{TKey, TValue}"/> with cookie.
        /// </summary>
        /// <param name="cookie">The cookie.</param>
        public DictionaryMarshaler(string cookie) : this((ArrayMarshaller<T, TMarshaler>)ArrayMarshaller<T, TMarshaler>.GetInstance(cookie))
        {

        }

        /// <summary>
        /// Initialize a new <see cref="Dictionary{TKey, TValue}"/> with a specified <see cref="ArrayMarshaller{T, TMarshaler}"/>.
        /// </summary>
        /// <param name="arrayMarshaler">The underlined array marshaller.</param>
        public DictionaryMarshaler(ArrayMarshaller<T,TMarshaler> arrayMarshaler)
        {
            _arrayMarshaler = arrayMarshaler;
        }

        /// <summary>
        /// Initialize a new <see cref="Dictionary{TKey, TValue}"/> with a specified <see cref="ArrayMarshaller{T, TMarshaler}"/>.
        /// </summary>
        /// <param name="itemMarshaler">The underlined array marshaller.</param>
        public DictionaryMarshaler(TMarshaler itemMarshaler) : this(new ArrayMarshaller<T,TMarshaler>(itemMarshaler))
        {

        }

        /// <inheritdoc/>
        public override void CleanUpManagedData(IReadOnlyDictionary<T, T> managedObj)
        {

        }

        /// <inheritdoc/>
        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            _arrayMarshaler.CleanUpNativeData(pNativeData);
        }

        /// <inheritdoc/>
        public override int GetNativeDataSize()
        {
            return _arrayMarshaler.GetNativeDataSize();
        }

        /// <inheritdoc/>
        public override IntPtr MarshalManagedToNative(IReadOnlyDictionary<T,T> managedObj)
        {
            if (managedObj == null)
            {
                return IntPtr.Zero;
            }

            var array = managedObj.SelectMany((kv) => new[] { kv.Key, kv.Value }).ToArray();
            return _arrayMarshaler.MarshalManagedToNative(array);
        }

        /// <inheritdoc/>
        public override IReadOnlyDictionary<T,T> MarshalNativeToManaged(IntPtr pNativeData)
        {
            var array = _arrayMarshaler.MarshalNativeToManaged(pNativeData).AsEnumerable();
            var dic = new Dictionary<T,T>();
            var enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T key, value;
                key = enumerator.Current;
                if (!enumerator.MoveNext())
                {
                    break;
                }
                value = enumerator.Current;
                dic.Add(key, value);
            }
             return new ReadOnlyDictionary<T, T>(dic);
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }

        public static DictionaryMarshaler<T,TMarshaler> GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
