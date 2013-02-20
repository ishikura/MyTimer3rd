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
        volatile private static bool IsCreated = false;
        private static TimerValueListModel _timerValueListModel;

        private static TimerListSettings _timerListSettings;

        private static List<TimeSpan> TimerValueList = new List<TimeSpan>() { };

        private static object syncObj = new object();

        public static TimerValueListModel Instance()
        {
            if (IsCreated)
            {
                return _timerValueListModel;
            }
            else
            {
                _timerValueListModel = new TimerValueListModel();

                _timerListSettings = new TimerListSettings();

                _timerListSettings.Reload();

                if (_timerListSettings.TimerList == null)
                {
                    // リストが読めない場合は3分を1個だけ設定
                    TimerValueList.Add(TimeSpan.Parse("0, 3, 0"));
                }
                else
                {
                    foreach (string ts in _timerListSettings.TimerList)
                    {
                        TimerValueList.Add(TimeSpan.Parse(ts));
                    }
                }

                IsCreated = true;

                return _timerValueListModel;
            }
        }

        public List<TimeSpan> getEditTimerValueList()
        {
            lock (syncObj)
            {
            }
            return TimerValueList;
        }

        public bool UpdateEditTimerValueList(List<TimeSpan> timerList)
        {
            
            List<string> saveTimeListStr = new List<string>();

            lock (syncObj)
            {
                TimerValueList = timerList;
            }

            foreach (TimeSpan ts in timerList)
            {
                saveTimeListStr.Add(ts.ToString());
            }

            _timerListSettings.TimerList = saveTimeListStr;
            
            _timerListSettings.Save();

            RaisePropertyChanged("TimerValueListModel");

            return true;
        }

    }
}
