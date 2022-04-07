using System;
using System.Collections.Generic;
using System.Text;
using IOSLib.PropertyList.Native;
using static IOSLib.PropertyList.Native.Plist;

namespace IOSLib.PropertyList
{
    /// <summary>
    /// Represent a plist node that contain <see cref="DateTime"/> value.
    /// </summary>
    public sealed partial class PlistDate : PlistValueNode<DateTime>
    {

        /// <summary>
        /// Create <see cref="DateTime"/> plist node from an existing handle.
        /// </summary>
        /// <param name="handle">The <see cref="PlistHandle"/> of type <see cref="PlistType.Date"/> to wrap.</param>
        public PlistDate(PlistHandle handle) : base(handle)
        {

        }
        /// <summary>
        /// Initialize a new Plist docs with the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value.</param>
        public PlistDate(DateTime date) : this((DateValue)date)
        {

        }

        private PlistDate(DateValue date) : base(plist_new_date(date.sec, date.usec))
        {

        }


        private static readonly DateTime baseDateTime = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <inheritdoc/>
        public override DateTime Value
        {
            get
            {
                plist_get_date_val(Handle, out var sec, out var microsec);
                return new DateValue(sec, microsec);
            }
            set
            {
                (int sec, int microsec) = (DateValue)value;
                plist_set_date_val(Handle, sec, microsec);
            }
        }
    }
}

