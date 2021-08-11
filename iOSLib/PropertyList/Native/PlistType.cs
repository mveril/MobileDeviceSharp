namespace IOSLib.PropertyList.Native
{
    /// <summary>
    /// The enumeration of plist node types.
    /// </summary>
    public enum PlistType : int
    {

        Boolean = 0,

        Uint = 1,

        Real = 2,

        String = 3,

        Array = 4,

        Dict = 5,

        Date = 6,

        Data = 7,

        Key = 8,

        Uid = 9,

        /// <summary>
        /// No type 
        /// </summary>
        None = 10,
    }
}