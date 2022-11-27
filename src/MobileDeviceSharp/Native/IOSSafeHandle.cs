using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace MobileDeviceSharp.Native
{
    public abstract class IOSHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected IOSHandle() : this(IntPtr.Zero)
        {

        }


        protected IOSHandle(IntPtr handle) : this(handle, true)
        {

        }

        protected IOSHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            SetHandle(handle);
        }

        protected IOSHandle(bool ownsHandle) : this(IntPtr.Zero, ownsHandle)
        {

        }

        protected virtual bool CanBeReleased() => true;
    }
}
