using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

using Livet;

namespace MyTimer3rd
{
    public class MyUtil
    {
        public static string TimeSpanTo24hStr(TimeSpan value)
        {
            int hh = (int)value.TotalHours;
            return hh.ToString("D2") + value.ToString(@"\:mm\:ss");
        }

    }
}
