namespace MobileDeviceSharp.PropertyList.Native
{
    /// <summary>
    /// Represents the enumeration of possible types of a node in a Property List (plist) file.
    /// </summary>
    public enum PlistType : int
    {
        /// <summary>
        /// A boolean value (true or false).
        /// </summary>
        Boolean = 0,

        /// <summary>
        /// An unsigned integer value.
        /// </summary>
        Uint = 1,

        /// <summary>
        /// A floating-point value.
        /// </summary>
        Real = 2,

        /// <summary>
        /// A string value.
        /// </summary>
        String = 3,

        /// <summary>
        /// An array of nodes.
        /// </summary>
        Array = 4,

        /// <summary>
        /// A dictionary of nodes, where each node has a unique key.
        /// </summary>
        Dict = 5,

        /// <summary>
        /// A date/time value.
        /// </summary>
        Date = 6,

        /// <summary>
        /// Binary data.
        /// </summary>
        Data = 7,

        /// <summary>
        /// A key value in a dictionary node.
        /// </summary>
        Key = 8,

        /// <summary>
        /// An unsigned integer value used as a unique identifier.
        /// </summary>
        Uid = 9,

        /// <summary>
        /// Indicates that no type has been assigned to the node.
        /// </summary>
        None = 10,
    }
}
