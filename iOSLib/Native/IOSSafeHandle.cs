using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace IOSLib.Native
{
    public abstract class IOSHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        protected IOSHandle() : this(IntPtr.Zero)
        {

        }


        protected IOSHandle(IntPtr Handle) : this(Handle,true)
        {
            
        } 

        protected IOSHandle(IntPtr Handle, bool ownsHandle) : base(ownsHandle)
        {
            this.SetHandle(handle);
        }

        protected IOSHandle(bool ownsHandle) : this(IntPtr.Zero, ownsHandle)
        {

        }

    }
}
