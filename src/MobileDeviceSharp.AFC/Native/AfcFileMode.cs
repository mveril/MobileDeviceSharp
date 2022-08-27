namespace MobileDeviceSharp.AFC.Native
{
    /// <summary>
    /// Flags for afc_file_open
    /// </summary>
    public enum AFCFileMode : int
    {

        FopenRdonly = 1,

        FopenRw = 2,

        FopenWronly = 3,

        FopenWr = 4,

        FopenAppend = 5,

        /// <summary>
        /// a+  O_RDWR   | O_APPEND | O_CREAT
        /// </summary>
        FopenRdappend = 6,
    }
}
