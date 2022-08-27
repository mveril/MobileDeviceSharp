using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.Native;

namespace MobileDeviceSharp
{
    public enum PairingState
    {
        Success = LockdownError.Success,

        PasswordProtected = LockdownError.PasswordProtected,

        UserDeniedPairing = LockdownError.UserDeniedPairing,

        PairingDialogResponsePending = LockdownError.PairingDialogResponsePending,
    }
}
