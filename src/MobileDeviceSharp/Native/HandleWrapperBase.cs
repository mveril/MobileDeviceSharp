﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDeviceSharp.Native
{
    /// <summary>
    /// Represent a base class to create a object oriented wrapper arond <see cref="IOSHandle"/> features
    /// </summary>
    /// <typeparam name="T">IOSHanlde type</typeparam>
    public abstract class IOSHandleWrapperBase<T> : IDisposable where T : IOSHandle, new()
    {

        /// <summary>
        /// Base constructor to wrap already existing Handle
        /// </summary>
        /// <param name="handle">The handle to wrap</param>
        protected IOSHandleWrapperBase(T handle)
        {
            Handle = handle;
        }
        /// <summary>
        /// Base constructor with without already created handle
        /// </summary>
        protected IOSHandleWrapperBase()
        {
            Handle = new T();
        }
        /// <summary>
        /// The wrapped handle
        /// </summary>
        public T Handle { get; protected set; }

        /// <summary>
        /// Virtual method runed on close
        /// </summary>
        protected virtual void OnClose() { }

        /// <summary>
        ///  Close the wrapper
        /// </summary>
        public void Close()
        {
            OnClose();
            Dispose();
        }

        /// <summary>
        /// Return true if the <see cref="Handle"/> is closed
        /// </summary>
        public bool IsClosed => Handle.IsClosed;

        private bool _disposedValue;

        /// <summary>
        /// Dispose method for releasing managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates if the method is being called from the Dispose method or from the finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Handle.Dispose();
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Finalizer for releasing unmanaged resources.
        /// </summary>
        ~IOSHandleWrapperBase()
        {
            Dispose(disposing: false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
