using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MobileDeviceSharp.Native
{
    public class UTF8DecomposedMarshaler : CustomMashaler<string>
    {
        private static readonly Lazy<UTF8DecomposedMarshaler> s_static_instance = new();

        private readonly UTF8Marshaler _utf8Marshaler;


        public UTF8DecomposedMarshaler(UTF8Marshaler utf8Marshaler)
        {
            _utf8Marshaler = utf8Marshaler;
        }

        public UTF8DecomposedMarshaler()
        {
            _utf8Marshaler = UTF8Marshaler.GetInstance();
        }

        public override unsafe IntPtr MarshalManagedToNative(string managedObj)
        {
            var normalized = managedObj.Normalize(NormalizationForm.FormD);
            return _utf8Marshaler.MarshalManagedToNative(normalized);
        }

        public override string MarshalNativeToManaged(IntPtr pNativeData)
        {
            var managedObj = _utf8Marshaler.MarshalNativeToManaged(pNativeData);
            var normalized = managedObj.Normalize(NormalizationForm.FormC);
            return normalized;
        }

        public override void CleanUpNativeData(IntPtr pNativeData)
        {
            _utf8Marshaler.CleanUpNativeData(pNativeData);
        }

        public override void CleanUpManagedData(string managedObj)
        {
            _utf8Marshaler.CleanUpManagedData(managedObj);
        }

        public override int GetNativeDataSize()
        {
            return _utf8Marshaler.GetNativeDataSize();
        }

        public static UTF8DecomposedMarshaler GetInstance()
        {
            return s_static_instance.Value;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return s_static_instance.Value;
        }
    }
}
