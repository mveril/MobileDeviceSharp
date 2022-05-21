using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.Native;

namespace IOSLib
{
    public enum PairingState
    {
        Success = LockdownError.Success,

        PasswordProtected = LockdownError.PasswordProtected,

        UserDeniedPairing = LockdownError.UserDeniedPairing,

        PairingDialogResponsePending = LockdownError.PairingDialogResponsePending,
    }
}
