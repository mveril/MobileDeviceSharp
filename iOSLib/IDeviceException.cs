using IOSLib.Native;

namespace IOSLib
{
    class IDeviceException : MobileDeviceException
    {
        IDeviceException() :base()
        {

        }

        IDeviceException(iDeviceError errorCode) : base(GetMessageForHResult(errorCode),(int)errorCode)
        {

        }

        IDeviceException(string message,iDeviceError errorCode) : base(message, (int)errorCode)
        {

        }
    }
}
