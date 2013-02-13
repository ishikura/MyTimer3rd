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
    public class TimerValueEditWindowViewModel : ViewModel
    {
        private TimerValueListModel _timerValueListModel;

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


        #region EditTimerValueList変更通知プロパティ
        private List<TimeSpan> _EditTimerValueList;

        public List<TimeSpan> EditTimerValueList
        {
            get
            { return _EditTimerValueList; }
            set
            { 
                if (_EditTimerValueList == value)
                    return;
                _EditTimerValueList = value;
                RaisePropertyChanged("EditTimerValueList");
            }
        }
        #endregion


        #region H10Value変更通知プロパティ
        private int _H10Value;

        public int H10Value
        {
            get
            { return _H10Value; }
            set
            { 
                if (_H10Value == value)
                    return;
                _H10Value = value;
                RaisePropertyChanged("H10Value");
            }
        }
        #endregion
        #region H01Value変更通知プロパティ
        private int _H01Value;

        public int H01Value
        {
            get
            { return _H01Value; }
            set
            { 
                if (_H01Value == value)
                    return;
                _H01Value = value;
                RaisePropertyChanged("H01Value");
            }
        }
        #endregion
        #region M10Value変更通知プロパティ
        private int _M10Value;

        public int M10Value
        {
            get
            { return _M10Value; }
            set
            { 
                if (_M10Value == value)
                    return;
                _M10Value = value;
                RaisePropertyChanged("M10Value");
            }
        }
        #endregion
        #region M01Value変更通知プロパティ
        private int _M01Value;

        public int M01Value
        {
            get
            { return _M01Value; }
            set
            { 
                if (_M01Value == value)
                    return;
                _M01Value = value;
                RaisePropertyChanged("M01Value");
            }
        }
        #endregion
        #region S10Value変更通知プロパティ
        private int _S10Value;

        public int S10Value
        {
            get
            { return _S10Value; }
            set
            { 
                if (_S10Value == value)
                    return;
                _S10Value = value;
                RaisePropertyChanged("S10Value");
            }
        }
        #endregion
        #region S01Value変更通知プロパティ
        private int _S01Value;

        public int S01Value
        {
            get
            { return _S01Value; }
            set
            { 
                if (_S01Value == value)
                    return;
                _S01Value = value;
                RaisePropertyChanged("S01Value");
            }
        }
        #endregion


        #region SelectedItem変更通知プロパティ
        private TimeSpan _SelectedItem;

        public TimeSpan SelectedItem
        {
            get
            { return _SelectedItem; }
            set
            { 
                if (_SelectedItem == value)
                    return;
                _SelectedItem = value;

                SeparateAndSetSelectedValue();

                RaisePropertyChanged("SelectedItem");
            }
        }
        #endregion



        #region 右クリックでの数値選択Command
        /// <summary>
        /// 選択されたケタ数を判断してそのケタの数値を変更
        /// </summary>
        /// <param name="parameter"></param>
        private ListenerCommand<string> _ContextSelectNumberCommand;
        public ListenerCommand<string> ContextSelectNumberCommand
        {
            get
            {
                if (_ContextSelectNumberCommand == null)
                {
                    _ContextSelectNumberCommand = new ListenerCommand<string>(ContextSelectNumber);
                }
                return _ContextSelectNumberCommand;
            }
        }

        public void ContextSelectNumber(string parameter)
        {
            var splitStr = parameter.Split('_');
            var selectedValue = Convert.ToInt32(splitStr[1]);

            switch(splitStr[0])
            {
                case "H10":
                    H10Value = selectedValue;
                    break;
                case "H01":
                    H01Value = selectedValue;
                    break;
                case "M10":
                    M10Value = selectedValue;
                    break;
                case "M01":
                    M01Value = selectedValue;
                    break;
                case "S10":
                    S10Value = selectedValue;
                    break;
                case "S01":
                    S01Value = selectedValue;
                    break;
                default:
                    break;
            }
        }
        #endregion






        public void Initialize()
        {
            _timerValueListModel = TimerListFactory.Create();
            EditTimerValueList = _timerValueListModel.getEditTimerValueList();
        }

        /// <summary>
        /// ComboBoxで選択したタイマ値を一桁ずつ設定
        /// </summary>
        private void SeparateAndSetSelectedValue()
        {
            String tmpStrings = SelectedItem.ToString(@"hh\:mm\:ss");
            H10Value = int.Parse(tmpStrings[0].ToString());
            H01Value = int.Parse(tmpStrings[1].ToString());
            // [2]は":"の部分
            M10Value = int.Parse(tmpStrings[3].ToString());
            M01Value = int.Parse(tmpStrings[4].ToString());
            // [5]は":"の部分
            S10Value = int.Parse(tmpStrings[6].ToString());
            S01Value = int.Parse(tmpStrings[7].ToString());
        }
    }
}
