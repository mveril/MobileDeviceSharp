using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp.PropertyList.Native
{
    internal class PlistHandleNotOwnedMarshaler : CustomMashaler<PlistHandle>
    {
        private static readonly Lazy<PlistHandleNotOwnedMarshaler> s_static_instance = new();

        public override void CleanUpManagedData(PlistHandle managedObj)
        {

        }
        public override void CleanUpNativeData(IntPtr pNativeData)
        {

        }
        public override int GetNativeDataSize()
        {
            return -1;
        }
        public override IntPtr MarshalManagedToNative(PlistHandle managedObj)
        {
            return managedObj.DangerousGetHandle();
        }
        public override PlistHandle MarshalNativeToManaged(IntPtr pNativeData)
        {
            return new PlistHandle(pNativeData, false);
        }

        public static new ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }

        public static new PlistHandleNotOwnedMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }
    }
}
