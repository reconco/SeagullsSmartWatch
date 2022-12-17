using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Diagnostics;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>


    public partial class MainWindow : Window
    {
        public static WatchSetting setting = new WatchSetting();

        public static bool isMute = false;

        SeagullWatch currentWatch = null;
        SeagullWatch previousWatch = null;
        
        DispatcherTimer updateTimer = new DispatcherTimer();

        public SoundPlayer soundPlayer = new SoundPlayer();

        SettingWindow settingWindow = null;

        public bool CurrentWatchIsNotRun
        {
            get { return currentWatch.IsNotRun(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadSaveData();
            Reset();

            soundPlayer.LoadSoundFile(setting.notiSoundFile);
            ChangeWatchTypeText();

            Grid_MouseLeave(this, null);

            //timer.Dispatcher.Thread.Priority = System.Threading.ThreadPriority.AboveNormal;

            updateTimer.Interval = TimeSpan.FromSeconds(0.1);
            updateTimer.Tick += new EventHandler(UpdateTick);
        }
        private void UpdateTick(object sender, EventArgs e)
        {
            TimeSpan currentTime = currentWatch.CurrentTime;

            currentWatch.Update();
            UpdateNotification(currentTime);

            //메시지 출력중이 아니라면
            if (!usingDayTextToMessage)
            {
                if (currentTime.Days > 0)
                    day.Visibility = Visibility.Visible;
                else
                    day.Visibility = Visibility.Hidden;

                day.Text = currentTime.Days.ToString() + " 일";
            }

            time.Text = currentTime.Hours.ToString("D2") + ":" + currentTime.Minutes.ToString("D2") + ":" + currentTime.Seconds.ToString("D2");


            cancleResetButton.IsEnabled = 
                (previousWatch != null) && (currentWatch.GetType() == previousWatch.GetType());
        }

        public void ChangeWatchTypeText()
        {
            if(setting.hideWatchTypeText)
            {
                watchTypeText.Visibility = Visibility.Hidden;
                return;
            }

            watchTypeText.Visibility = Visibility.Visible;

            if (setting.watchType == WatchType.Stopwatch)
                watchTypeText.Text = "Stopwatch";
            else
                watchTypeText.Text = "Timer";
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonText = startButton.Content as string;
            if (buttonText == "시작")
            {
                startButton.Content = "일시정지";

                currentWatch.Start();
                updateTimer.Start();
            }
            else if (buttonText == "일시정지")
            {
                startButton.Content = "시작";
                soundPlayer.Stop();
                currentWatch.Stop();

                leftNotifyingStopWatch.Reset();
                usingDayTextToMessage = false;
                SetTextColor(setting.textColor);
                UpdateTick(this, null);
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        private void cancleResetButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToPreviosWatch();
        }

        private void CreateNewWatch()
        {
            if(currentWatch != null && !currentWatch.IsNotRun())
                previousWatch = currentWatch;

            if (setting.watchType == WatchType.Timer)
            {
                int h = setting.timer_hours;
                int m = setting.timer_minutes;
                int s = setting.timer_seconds;

                currentWatch = new SeagullTimer(this, new TimeSpan(h, m, s));
            }
            else
            {
                currentWatch = new SeagullStopWatch(this);
            }
        }

        public void Reset()
        {
            startButton.Content = "시작";

            CreateNewWatch();
            //currentWatch.Reset();
            usingDayTextToMessage = false;
            UpdateTick(this, null);
            updateTimer.Stop();

            leftNotifyingStopWatch.Reset();
            SetTextColor(setting.textColor);

            soundPlayer.Stop();
        }
        public void AdjustNotifyChanged()
        {
            usingDayTextToMessage = false;

            if(currentWatch is SeagullStopWatch)
            {
                ((SeagullStopWatch)currentWatch).SetToFirstNotifyPatternIndex();
                ((SeagullStopWatch)currentWatch).CalculateNextNotifyTime();
            }
        }

        public void ReturnToPreviosWatch()
        {
            currentWatch.Stop();
            soundPlayer.Stop();

            leftNotifyingStopWatch.Reset();
            SetTextColor(setting.textColor);

            currentWatch = previousWatch;
            previousWatch = null;
            UpdateTick(this, null);

            resetButton.Content = "초기화";
            startButton.Content = "시작";
        }

        public void SetBGColor(string colorStr)
        {
            Color newColor = (Color) ColorConverter.ConvertFromString(colorStr);
            SolidColorBrush newBrush = new SolidColorBrush(newColor);
            Background = newBrush;
        }

        public void SetTextColor(string colorStr)
        {
            Color newColor = (Color)ColorConverter.ConvertFromString(colorStr);
            SolidColorBrush newBrush = new SolidColorBrush(newColor);
            watchTypeText.Foreground = newBrush;
            day.Foreground = newBrush;
            time.Foreground = newBrush;
        }

        private void settingButton_Click(object sender, RoutedEventArgs e)
        {
            if (settingWindow == null)
            {
                settingWindow = new SettingWindow(this);
                settingWindow.Show();
                settingWindow.Closed += SettingWindow_Closed;
            }
            else
                settingWindow.Focus();
        }

        private void SettingWindow_Closed(object sender, EventArgs e)
        {
            settingWindow = null;
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            startButton.Visibility = Visibility.Visible;
            resetButton.Visibility = Visibility.Visible;
            settingButton.Visibility = Visibility.Visible;
            muteButton.Visibility = Visibility.Visible;
            cancleResetButton.Visibility = Visibility.Visible;
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            startButton.Visibility = Visibility.Hidden;
            resetButton.Visibility = Visibility.Hidden;
            settingButton.Visibility = Visibility.Hidden;
            muteButton.Visibility = Visibility.Hidden;
            cancleResetButton.Visibility = Visibility.Hidden;
        }

        private void muteButton_Click(object sender, RoutedEventArgs e)
        {
            string muteText = muteButton.Content as string;

            if(muteText == "음소거")
            {
                muteButton.Content = "음소거 해제";
                isMute = true;
                soundPlayer.Stop();
            }
            else if (muteText == "음소거 해제")
            {
                muteButton.Content = "음소거";
                isMute = false;
            }

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (settingWindow != null)
            {
                MessageBoxResult result = MessageBox.Show("열려있는 창이 있습니다. 갈매기 워치를 종료하시겠습니까?", "갈매기 워치 종료", MessageBoxButton.YesNo);
                
                if(result == MessageBoxResult.No)
                    e.Cancel = true;
                else
                {
                    if(settingWindow != null)
                        settingWindow.ForceClose();
                }
            }
        }
    }
}
