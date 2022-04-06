using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace IOSLib.Native
{
    public abstract class CustomMashaler<T> : ICustomMarshaler
    {
        void ICustomMarshaler.CleanUpManagedData(object ManagedObj)
        {
            CleanUpManagedData((T)ManagedObj);
        }



        public abstract void CleanUpManagedData(T managedObj);

        public abstract void CleanUpNativeData(IntPtr pNativeData);

        public abstract int GetNativeDataSize();

        IntPtr ICustomMarshaler.MarshalManagedToNative(object ManagedObj)
        {
            return MarshalManagedToNative((T)ManagedObj);
        }

        public abstract IntPtr MarshalManagedToNative(T managedObj);

        object ICustomMarshaler.MarshalNativeToManaged(IntPtr pNativeData)
        {
            return MarshalNativeToManaged(pNativeData);
        }

        public abstract T MarshalNativeToManaged(IntPtr pNativeData);
    }
}
