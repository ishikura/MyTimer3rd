﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Livet;

namespace MyTimer3rd.Models
{
    public class TimerValueListModel : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */
        public static List<TimeSpan> TimerValueList;

    }
}