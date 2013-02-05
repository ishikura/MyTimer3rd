using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Livet;

namespace MyTimer3rd.Models
{
    public class TimerModel : NotificationObject
    {
        public enum TimerStatus { Init, CountDown, Pause, CountUp, };

        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        #region NowTimerStatus変更通知プロパティ
        private TimerStatus _NowTimerStatus = TimerStatus.Init;

        public TimerStatus NowTimerStatus
        {
            get
            { return _NowTimerStatus; }
            set
            { 
                if (_NowTimerStatus == value)
                    return;
                _NowTimerStatus = value;
                RaisePropertyChanged("NowTimerStatus");
            }
        }
        #endregion      

        #region TimerRemainValue変更通知プロパティ
        private TimeSpan _TimerRemainValue = new TimeSpan(0, 3, 0);

        public TimeSpan TimerRemainValue
        {
            get
            { return _TimerRemainValue; }
            set
            { 
                if (_TimerRemainValue == value)
                    return;
                _TimerRemainValue = value;
                RaisePropertyChanged("TimerRemainValue");
            }
        }
        #endregion

        private static int _intervalMsec = 100;
        private static TimeSpan initialTimerValue = new TimeSpan(0, 3, 0);

        private static DateTime endTime;

        private static bool isPause = false;

        private static Task countdownTask;

        //呼び出し元に残り時間を返す
        public static Action<TimeSpan> CountdownCallback;

        public void setCallback(Action<TimeSpan> act)
        {
            CountdownCallback = act;
        }

        // カウントダウン時のインターバル設定（10mSec～60Sec）
        public int IntervalMsec
        {
            get { return _intervalMsec; }
        }
        public bool SetInterval(int intervalMsec)
        {
            if (intervalMsec >= 10 && intervalMsec <= (60 * 1000))
            {
                _intervalMsec = intervalMsec;
                return true;
            }
            else
            {
                return false;
            }
        }


        // 最大で99:59:59まで（主に画面表示の都合による）（ここでnew使うのあり？）
        public bool SetTimerValue(TimeSpan timerValue)
        {
            if (timerValue > TimeSpan.Zero && timerValue <= new TimeSpan(99, 59, 59))
            {
                initialTimerValue = timerValue;
                TimerRemainValue = timerValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// カウントダウンSTARTorRESTART
        /// </summary>
        public void Start()
        {
            if (isPause)
            {
                endTime = DateTime.Now + TimerRemainValue;
            }
            else
            {
                //if (CountdownCallback == null)
                //{
                //    // TODO: コールバック未設定の場合の挙動
                //    return;
                //}
                //終了時刻を確定(RESTART時はPAUSE時の残り時間を使用)
                endTime = DateTime.Now + TimerRemainValue;

                if (countdownTask == null)
                {
                    countdownTask = new Task(countdownTaskImp);
                    countdownTask.Start();
                }
                else if (countdownTask.IsCompleted)
                {
                    countdownTask.Dispose();
                    countdownTask = new Task(countdownTaskImp);
                    countdownTask.Start();
                }
            }
            NowTimerStatus = TimerStatus.CountDown;
            isPause = false;
        }
        /// <summary>
        /// 一時停止（残り時間そのまま）
        /// </summary>
        public void Pause()
        {
            isPause = true;
            NowTimerStatus = TimerStatus.Pause;

            //現在の残り時間を保持
            TimerRemainValue = endTime - DateTime.Now;

            //countdownTask.;
            //tokenを使用したキャンセル処理は例外を使用する必要があるので、別の宿題とする。
            //フラグだらけとどっちがいいのか…？
        }

        /// <summary>
        /// RESET（終了時刻を変更してカウントダウンは継続）
        /// </summary>
        public void Reset()
        {
            endTime = DateTime.Now + initialTimerValue;
            TimerRemainValue = initialTimerValue;
        }

        //カウントダウン
        private void countdownTaskImp()
        {
            while (true)
            {
                if (!isPause)
                {
                    if ((endTime - DateTime.Now) > TimeSpan.Zero)
                    {
                        //残り時間を返す
                        TimerRemainValue = endTime - DateTime.Now;
                    }
                    else
                    {
                        //TIMEUP!（…のはず）
                        NowTimerStatus = TimerStatus.CountUp;
                        TimerRemainValue = TimeSpan.Zero;

                        break;
                    }
                }
                Thread.Sleep(_intervalMsec);
            }
        }
    }
}
