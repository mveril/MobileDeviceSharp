using System;
using System.Collections.Generic;
using System.Text;
using MobileDeviceSharp.NotificationProxy;

namespace MobileDeviceSharp.NotificationProxy.Native
{
    /// <summary>
    /// Represent a struct containing a list of NotificationProxy events.
    /// </summary>
    public struct NotificationProxyEvents
    {

        /// <summary>
        /// Event that can be send with <see cref="NotificationProxySessionBase.RaiseEvent(string)"/>
        /// </summary>
        public struct Sendable
        {
            public const string NP_SYNC_WILL_START = "com.apple.itunes-mobdev.syncWillStart";
            public const string NP_SYNC_DID_START = "com.apple.itunes-mobdev.syncDidStart";
            public const string NP_SYNC_DID_FINISH = "com.apple.itunes-mobdev.syncDidFinish";
            public const string NP_SYNC_LOCK_REQUEST = "com.apple.itunes-mobdev.syncLockRequest";
        }
        /// <summary>
        /// Event that can be receved by with <see cref="NotificationProxySessionBase.ObserveNotification(string)"/> or <see cref="NotificationProxySessionBase.ObserveNotificationAsync(string)"/>.
        /// </summary>
        public struct Recevable
        {
        /// Event Can be receved
        public const string NP_SYNC_CANCEL_REQUEST = "com.apple.itunes-client.syncCancelRequest";
        public const string NP_SYNC_SUSPEND_REQUEST = "com.apple.itunes-client.syncSuspendRequest";
        public const string NP_SYNC_RESUME_REQUEST = "com.apple.itunes-client.syncResumeRequest";
        public const string NP_PHONE_NUMBER_CHANGED = "com.apple.mobile.lockdown.phone_number_changed";
        public const string NP_DEVICE_NAME_CHANGED = "com.apple.mobile.lockdown.device_name_changed";
        public const string NP_TIMEZONE_CHANGED = "com.apple.mobile.lockdown.timezone_changed";
        public const string NP_TRUSTED_HOST_ATTACHED = "com.apple.mobile.lockdown.trusted_host_attached";
        public const string NP_HOST_DETACHED = "com.apple.mobile.lockdown.host_detache";
        public const string NP_HOST_ATTACHED = "com.apple.mobile.lockdown.host_attached";
        public const string NP_REGISTRATION_FAILED = "com.apple.mobile.lockdown.registration_failed";
        public const string NP_ACTIVATION_STATE = "com.apple.mobile.lockdown.activation_state";
        public const string NP_BRICK_STATE = "com.apple.mobile.lockdown.brick_state";
        public const string NP_DISK_USAGE_CHANGED = "com.apple.mobile.lockdown.disk_usage_changed";
        public const string NP_DS_DOMAIN_CHANGED = "com.apple.mobile.data_sync.domain_changed";
        public const string NP_APP_INSTALLED = "com.apple.mobile.application_installed";
        public const string NP_APP_UNINSTALLED = "com.apple.mobile.application_uninstalled";
        public const string NP_DEV_IMAGE_MOUNTED = "com.apple.mobile.developer_image_mounted";
        public const string NP_ATTEMPTACTIVATION = "com.apple.springboard.attemptactivation";
        public const string NP_ITDBPREP_DID_END = "com.apple.itdbprep.notification.didEnd";
        public const string NP_LANGUAGE_CHANGED = "com.apple.language.changed";
        public const string NP_ADDRESS_BOOK_PREF_CHANGED = "com.apple.AddressBook.PreferenceChanged";
        }
    }
}
