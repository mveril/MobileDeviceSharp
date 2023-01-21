using System.Runtime.InteropServices;
using MobileDeviceSharp.PropertyList.Native;
using static MobileDeviceSharp.InstallationProxy.Native.InstallationProxy;

namespace MobileDeviceSharp.InstallationProxy
{
    internal abstract class OperationStatusContext
    {
        protected abstract void OnUpdateProgress(PlistHandle command, PlistHandle status);

        internal void ReportProgress(PlistHandle command, PlistHandle status)
        {
            
            if (TryGetError(status, out var errorName, out var errorDescription, out var errorCode))
            {
                OnExceptionCore(command, errorName, errorDescription, errorCode);
            }
            OnUpdateProgress(command, status);
            if (IsComplete(status))
            {
                OnCompleted(command, status);
            }
        }

        private static bool TryGetError(PlistHandle status, out string errorName, out string errorDescription, out ulong errorCode)
        {
            instproxy_status_get_error(status, out errorName, out errorDescription, out errorCode);
            return errorName is not null;
        }

        protected virtual void OnExceptionCore(PlistHandle command, string errorName, string errorDescription, ulong  errorCode)
        {
            OnException(new InstallationProxyOperationException(errorName, errorDescription, errorCode));
        }

        protected abstract void OnException(InstallationProxyOperationException exception);

        protected abstract void OnCompleted(PlistHandle command, PlistHandle status);

        internal static bool IsComplete(PlistHandle status)
        {
            instproxy_status_get_name(status, out string statusName);
            return statusName is not null && statusName.Equals("Complete", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
