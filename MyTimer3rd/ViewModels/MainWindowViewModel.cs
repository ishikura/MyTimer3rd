using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using MyTimer3rd.Models;

namespace MyTimer3rd.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private TimerModel _timerModel;

        public MainWindowViewModel()
        {
            #region Timerモデル生成および聞き耳設定
            _timerModel = new TimerModel();
            var listener = new PropertyChangedEventListener(_timerModel);

            listener.RegisterHandler("NowTimerStatus", (sender, e) =>
            {
                if (_timerModel.NowTimerStatus == TimerModel.TimerStatus.CountDown)
                {
                    StartPauseButtonContent = "PAUSE";
                    TimerValueListIsEnable = false;
                }
                else
                {
                    StartPauseButtonContent = "START";
                    TimerValueListIsEnable = true;
                }
            });

            listener.RegisterHandler("TimerRemainValue", (sender, e) =>
            {
                RemainTime = _timerModel.TimerRemainValue.ToString(@"hh\:mm\:ss\:fff");
            });
            this.CompositeDisposable.Add(listener);
            #endregion
        }

        /// <summary>
        /// 表示用プロパティ
        /// </summary>
        #region StartPauseButtonContent変更通知プロパティ
        private string _StartPauseButtonContent = "START";

        public string StartPauseButtonContent
        {
            get
            { return _StartPauseButtonContent; }
            set
            { 
                if (_StartPauseButtonContent == value)
                    return;
                _StartPauseButtonContent = value;
                RaisePropertyChanged("StartPauseButtonContent");
            }
        }
        #endregion
        #region RemainTime変更通知プロパティ
        private string _RemainTime = "00:00:00";

        public string RemainTime
        {
            get
            { return _RemainTime; }
            set
            { 
                if (_RemainTime == value)
                    return;
                _RemainTime = value;
                RaisePropertyChanged("RemainTime");
            }
        }
        #endregion
        #region TimerValueList変更通知プロパティ
        private List<TimeSpan> _TimerValueList
            = new List<TimeSpan>() {
                new TimeSpan(0, 1, 0),
                new TimeSpan(0, 3, 0),
                new TimeSpan(0, 5, 0),
                new TimeSpan(0,10, 0),};

        public List<TimeSpan> TimerValueList
        {
            get
            { return _TimerValueList; }
            set
            { 
                if (_TimerValueList == value)
                    return;
                _TimerValueList = value;
                RaisePropertyChanged("TimerValueList");
            }
        }
        #endregion


        #region TimerValueListIsEnable変更通知プロパティ
        private bool _TimerValueListIsEnable = true;

        public bool TimerValueListIsEnable
        {
            get
            { return _TimerValueListIsEnable; }
            set
            { 
                if (_TimerValueListIsEnable == value)
                    return;
                _TimerValueListIsEnable = value;
                RaisePropertyChanged("TimerValueListIsEnable");
            }
        }
        #endregion


        #region 選択タイマ値
        private TimeSpan _selectedTimerValue;
        public TimeSpan SelectedTimerValue // = new TimeSpan(0, 3, 0);
        {
            get { return _selectedTimerValue; }
            set
            {
                _selectedTimerValue = value;
                _timerModel.SetTimerValue(value);
                RemainTime = value.ToString(@"hh\:mm\:ss");
            }
        }
        #endregion


        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

         public void Initialize()
        {
        }
        
        ///Menuコマンド
        /// Closeコマンドを作ってみたけど、なぜかCloseしてくれなかった。
        /// Exit(0)も効かない。ココが呼ばれてない？
        //#region Menu_CloseCommand
        //private ViewModelCommand _Menu_CloseCommand;

        //public ViewModelCommand Menu_CloseCommand
        //{
        //    get
        //    {
        //        if (_Menu_CloseCommand == null)
        //        {
        //            _Menu_CloseCommand = new ViewModelCommand(Menu_Close);
        //        }
        //        return _Menu_CloseCommand;
        //    }
        //}

        //public void Menu_Close()
        //{
        //    Environment.Exit(0);
        //    Messenger.Raise(new WindowActionMessage("Close", WindowAction.Close));
        //}
        //#endregion

        //ボタンコマンド

        #region StartPauseButtonCommand
        private ViewModelCommand _StartPauseButtonCommand;

        public ViewModelCommand StartPauseButtonCommand
        {
            get
            {
                if (_StartPauseButtonCommand == null)
                {
                    _StartPauseButtonCommand = new ViewModelCommand(StartPauseButton, CanStartPauseButton);
                }
                return _StartPauseButtonCommand;
            }
        }

        public bool CanStartPauseButton()
        {
            return _timerModel.NowTimerStatus != TimerModel.TimerStatus.CountUp;
        }

        public void StartPauseButton()
        {
            if (_timerModel.NowTimerStatus == TimerModel.TimerStatus.CountDown)
            {
                _timerModel.Pause();
            }
            else
            {
                _timerModel.Start();
            }
        }
        #endregion

        #region ResetButtonCommand
        private ViewModelCommand _ResetButtonCommand;

        public ViewModelCommand ResetButtonCommand
        {
            get
            {
                if (_ResetButtonCommand == null)
                {
                    _ResetButtonCommand = new ViewModelCommand(ResetButton, CanResetButton);
                }
                return _ResetButtonCommand;
            }
        }

        public bool CanResetButton()
        {
            return true;
        }

        public void ResetButton()
        {
            _timerModel.Reset();
        }
        #endregion

    }
}
