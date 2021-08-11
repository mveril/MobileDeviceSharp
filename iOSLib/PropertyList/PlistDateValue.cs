﻿using System;
using System.Collections.Generic;
using System.Text;

namespace IOSLib.PropertyList
{

    public sealed partial class PlistDate
    {
        private record DateValue(int sec, int usec)
        {

            public static implicit operator DateTime(DateValue d)
            {
                (int sec, int microsec) = d;
                var milisec = microsec * 1e-3;
                return baseDateTime.AddSeconds(sec).AddMilliseconds(milisec);
            }
            public static explicit operator DateValue(DateTime dt)
            {
                var timespan = dt - baseDateTime;
                int sec = (int)timespan.TotalSeconds;
                timespan = timespan.Subtract(TimeSpan.FromSeconds(sec));
                int microsec = (int)(timespan.Ticks / (TimeSpan.TicksPerMillisecond * 1e-3));
                return new DateValue(sec, microsec);
            }
        }
    }
}