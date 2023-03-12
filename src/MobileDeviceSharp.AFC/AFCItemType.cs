using System;

namespace MobileDeviceSharp.AFC
{
    /// <summary>
    /// Represent the type of an AFC item.
    /// </summary>
    public readonly struct AFCItemType : IEquatable<AFCItemType>
    {
        /// <summary>
        /// Represent a regular file.
        /// </summary>
        public static AFCItemType File { get; } = new AFCItemType("S_IFREG");

        /// <summary>
        /// Represent a directory.
        /// </summary>
        public static AFCItemType Directory { get; } = new AFCItemType("S_IFDIR");

        /// <summary>
        /// Represent a symbolic link.
        /// </summary>
        public static AFCItemType SymbolicLink { get; } = new AFCItemType("S_IFLNK");

        internal string Name { get; }

        private AFCItemType(string AFCitemType)
        {
            if (AFCitemType == null) throw new ArgumentNullException(nameof(AFCitemType));
            if (AFCitemType.Length == 0) throw new ArgumentException(nameof(AFCitemType));

            Name = AFCitemType;
        }

        /// <summary>
        /// Creates a new AFCItemType instance.
        /// </summary>
        /// <remarks>If you plan to call this method frequently, please consider caching its result.</remarks>
        public static AFCItemType Create(string AFCitemType)
        {
            return new AFCItemType(AFCitemType);
        }

        public bool Equals(AFCItemType other)
        {
            return Equals(other.Name);
        }

        internal bool Equals(string? other)
        {
            return string.Equals(Name, other, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return obj is AFCItemType AFCitemType && Equals(AFCitemType);
        }

        public override int GetHashCode()
        {
            return Name == null ? 0 : Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name ?? string.Empty;
        }

        public static bool operator ==(AFCItemType left, AFCItemType right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AFCItemType left, AFCItemType right)
        {
            return !(left == right);
        }
    }
}
