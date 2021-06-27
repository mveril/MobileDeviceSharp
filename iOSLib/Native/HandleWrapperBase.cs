using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.Native
{
    public abstract class IOSHandleWrapperBase<T> : IDisposable where T : IOSHandle, new()
    {
        protected IOSHandleWrapperBase(T handle)
        {
            Handle = handle;
        }

        protected IOSHandleWrapperBase()
        {
            Handle = new T();
        }

        public T Handle { get; protected set; }

        protected virtual void OnClose() { }

        public void Close()
        {
            OnClose();
            Dispose();
        }

        public bool IsClosed => Handle.IsClosed;

        public void Dispose()
        {
            ((IDisposable)Handle).Dispose();
        }
    }
}
