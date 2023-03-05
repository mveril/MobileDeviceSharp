using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// A strong typed base type which implement ICustomMarshaler.
    /// </summary>
    /// <typeparam name="T">Type of the object to ashal</typeparam>
    public abstract class CustomMashaler<T> : ICustomMarshaler
    {
        void ICustomMarshaler.CleanUpManagedData(object ManagedObj)
        {
            CleanUpManagedData((T)ManagedObj);
        }

        /// <summary>
        /// Performs necessary cleanup of the managed data when it is no longer needed.
        /// </summary>
        /// <param name="managedObj">The managed object to be destroyed.</param>
        public abstract void CleanUpManagedData(T managedObj);

        /// <inheritdoc/>
        public abstract void CleanUpNativeData(IntPtr pNativeData);

        /// <inheritdoc/>
        public abstract int GetNativeDataSize();

        IntPtr ICustomMarshaler.MarshalManagedToNative(object ManagedObj)
        {
            return MarshalManagedToNative((T)ManagedObj);
        }

        /// <summary>
        /// Converts the managed data to unmanaged data.
        /// </summary>
        /// <param name="managedObj">The managed object to be converted.</param>
        /// <returns>A pointer to the COM view of the managed object.</returns>
        public abstract IntPtr MarshalManagedToNative(T managedObj);

        object ICustomMarshaler.MarshalNativeToManaged(IntPtr pNativeData)
        {
            return MarshalNativeToManaged(pNativeData);
        }

        /// <summary>
        /// Converts the unmanaged data to managed data.
        /// </summary>
        /// <param name="pNativeData">A pointer to the unmanaged data to be wrapped.</param>
        /// <returns>An object that represents the managed view of the COM data.</returns>
        public abstract T MarshalNativeToManaged(IntPtr pNativeData);
    }
}
