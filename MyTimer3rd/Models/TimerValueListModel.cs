using System;
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
        private static bool IsCreated = false;
        private static TimerValueListModel _timerValueListModel;

        private  static List<TimeSpan> TimerValueList;
        private  static List<TimeSpan> EditTimerValueList;

        public static TimerValueListModel Instance()
        {
            if (IsCreated)
            {
                return _timerValueListModel;
            }
            else
            {
                _timerValueListModel = new TimerValueListModel();
                IsCreated = true;
                TimerValueList = 
                new List<TimeSpan>() {
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 3, 0),
                new TimeSpan(0, 5, 0),
                new TimeSpan(0,10, 0),
                new TimeSpan(0,25, 0),
                };

                EditTimerValueList = TimerValueList;

                return _timerValueListModel;
            }
        }

        public List<TimeSpan> getEditTimerValueList()
        {
            return EditTimerValueList;
        }

        public bool UpdateEditTimerValueList(List<TimeSpan> timerList)
        {
            TimerValueList = timerList;
            EditTimerValueList = timerList;

            return true;
        }

    }
}
