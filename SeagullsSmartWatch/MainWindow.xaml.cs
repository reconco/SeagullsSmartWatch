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

        DispatcherTimer updateTimer = new DispatcherTimer();

        public SoundPlayer soundPlayer = new SoundPlayer();

        SettingWindow settingWindow = null;


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
            updateTimer.Tick += new EventHandler(updateTick);
        }
        private void updateTick(object sender, EventArgs e)
        {
            TimeSpan currentTime = stopWatch.Elapsed;

            if (setting.watchType == WatchType.Timer)
                currentTime = TimerTargetTime - stopWatch.Elapsed;

            updateNotifycation(currentTime);

            if (currentTime.Days > 0)
                day.Visibility = Visibility.Visible;
            else
                day.Visibility = Visibility.Hidden;


            day.Text = currentTime.Days.ToString() + " 일";
            time.Text = currentTime.Hours.ToString("D2") + ":" + currentTime.Minutes.ToString("D2") + ":" + currentTime.Seconds.ToString("D2");
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

                stopWatch.Start();
                updateTimer.Start();
            }
            else if (buttonText == "일시정지")
            {
                startButton.Content = "시작";
                soundPlayer.Stop();
                stopWatch.Stop();

                leftNotifyStopWatch.Reset();
                SetTextColor(setting.textColor);
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            startButton.Content = "시작";

            stopWatch.Reset();
            updateTick(this, null);
            updateTimer.Stop();

            if (setting.watchType == WatchType.Timer)
            {
                int h = setting.timer_hours;
                int m = setting.timer_minutes;
                int s = setting.timer_seconds;
                TimerTargetTime = new TimeSpan(h, m, s);
                updateTick(this, null);
            }

            nextNotifyTime = new TimeSpan(setting.useNotify_hours, setting.useNotify_minutes, setting.useNotify_seconds);
            leftNotifyStopWatch.Reset();
            SetTextColor(setting.textColor);

            soundPlayer.Stop();
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
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            startButton.Visibility = Visibility.Hidden;
            resetButton.Visibility = Visibility.Hidden;
            settingButton.Visibility = Visibility.Hidden;
            muteButton.Visibility = Visibility.Hidden;
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
    }
}
