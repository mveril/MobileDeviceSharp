namespace MobileDeviceSharp.AFC.Native
{
    /// <summary>
    /// Flags for afc_file_open
    /// </summary>
    internal enum AFCFileMode : int
    {
        /// <summary>
        /// Read only.
        /// O_RDONLY. (r)
        /// </summary>
        FopenRdonly = 1,

        /// <summary>
        /// Read and write.
        /// O_RDWR | O_CREAT. (r+)
        /// </summary>
        FopenRw = 2,

        /// <summary>
        /// Write only, file is created or truncated.
        /// O_WRONLY | O_CREAT | O_TRUNC. (w)
        /// </summary>
        FopenWronly = 3,

        /// <summary>
        /// Read and write, file is created or truncated.
        /// O_RDWR | O_CREAT | O_TRUNC. (w+)
        /// </summary>
        FopenWr = 4,

        /// <summary>
        /// Write only, file is created or opened in append mode.
        /// O_WRONLY | O_APPEND | O_CREAT. (a)
        /// </summary>
        FopenAppend = 5,

        /// <summary>
        /// Read and write, file is created or opened in append mode.
        /// O_RDWR | O_APPEND | O_CREAT. (a+)
        /// </summary>
        FopenRdappend = 6,
    }
}
