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
        public static readonly string SAVE_FILE_PATH = "setting.save";

        public static SaveFile saveFile = new SaveFile(SAVE_FILE_PATH);
        public static WatchSetting setting = new WatchSetting();

        public static bool isMute = false;

        DispatcherTimer updateTimer = new DispatcherTimer();
        Stopwatch stopWatch = new Stopwatch();
        public TimeSpan TimerTargetTime { get; set; } = new TimeSpan();
        TimeSpan nextNotifyTime = new TimeSpan();
        Stopwatch leftNotifyStopWatch = new Stopwatch();

        public SoundPlayer soundPlayer = new SoundPlayer();

        SettingWindow settingWindow = null;

        public TimeSpan NextNotifyTime { get { return nextNotifyTime; } }
        public bool WatchHadStart { get { return (stopWatch.ElapsedMilliseconds > 0); } }

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

        private void LoadSaveData()
        {
            if (saveFile.OpenSaveFile())
            {
                setting.watchType = (WatchType) saveFile.GetData("WatchType",0);

                setting.hideWatchTypeText = (bool)saveFile.GetData("ShowWatchType", false);
                setting.bgColor = (string)saveFile.GetData("bgColor", "#FFFFFFFF");
                setting.textColor = (string)saveFile.GetData("textColor", "#FF000000");

                setting.useNotifySound = (bool)saveFile.GetData("useNotifySound", false);
                setting.notiSoundFile = (string)saveFile.GetData("notifySoundFile", "Sounds\\BeepBeepBeep.wav");

                setting.notifyTextColor = (string)saveFile.GetData("notifyTextColor", "#FFFF0000");
                setting.notifyTextTime = (int)saveFile.GetData("NotifyTextTime", 0);

                setting.useNotify = (bool)saveFile.GetData("UseNotify", false);
                setting.useNotify_hours = (int)saveFile.GetData("UseNotify_H", 0);
                setting.useNotify_minutes = (int)saveFile.GetData("UseNotify_M", 0);
                setting.useNotify_seconds = (int)saveFile.GetData("UseNotify_S", 0);

                setting.timer_hours = (int)saveFile.GetData("Timer_H", 0);
                setting.timer_minutes = (int)saveFile.GetData("Timer_M", 0);
                setting.timer_seconds = (int)saveFile.GetData("Timer_S", 0);

            }
            else
                SaveSettingData();

            if (setting.watchType == WatchType.Timer)
            {
                int h = setting.timer_hours;
                int m = setting.timer_minutes;
                int s = setting.timer_seconds;
                TimerTargetTime = new TimeSpan(h, m, s);
                updateTick(this, null);
            }

        }

        public static void SaveSettingData()
        {
            saveFile.SetData("WatchType", (int)setting.watchType);
            saveFile.SetData("ShowWatchType", setting.hideWatchTypeText);
            saveFile.SetData("bgColor", setting.bgColor);
            saveFile.SetData("textColor", setting.textColor);

            saveFile.SetData("useNotifySound", setting.useNotifySound);
            saveFile.SetData("notifySoundFile", setting.notiSoundFile);

            saveFile.SetData("notifyTextColor", setting.notifyTextColor);
            saveFile.SetData("NotifyTextTime", setting.notifyTextTime);

            saveFile.SetData("UseNotify", setting.useNotify);
            saveFile.SetData("UseNotify_H", setting.useNotify_hours);
            saveFile.SetData("UseNotify_M", setting.useNotify_minutes);
            saveFile.SetData("UseNotify_S", setting.useNotify_seconds);

            saveFile.SetData("Timer_H", setting.timer_hours);
            saveFile.SetData("Timer_M", setting.timer_minutes);
            saveFile.SetData("Timer_S", setting.timer_seconds);

            saveFile.SaveThisFile();
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

        private void updateNotifycation(TimeSpan currentTime)
        {
            if (setting.watchType == WatchType.Stopwatch &&
                stopWatch.IsRunning)
            {
                if((nextNotifyTime - currentTime).TotalSeconds <= 0)
                {
                    if (setting.useNotify)
                    {
                        SetTextColor(setting.notifyTextColor);
                        leftNotifyStopWatch.Start();
                        nextNotifyTime += new TimeSpan(setting.useNotify_hours, setting.useNotify_minutes, setting.useNotify_seconds);

                        if(setting.useNotifySound)
                            soundPlayer.Play();
                    }
                }
            }
            else if (setting.watchType == WatchType.Timer &&
                setting.useNotify && stopWatch.IsRunning)
            {
                if (currentTime.TotalSeconds <= 0)
                {
                    SetTextColor(setting.notifyTextColor);
                    leftNotifyStopWatch.Start();
                    stopWatch.Stop();

                    if (setting.useNotifySound)
                        soundPlayer.Play();
                }

            }

            if(leftNotifyStopWatch.Elapsed.TotalSeconds >= setting.notifyTextTime)
            {
                SetTextColor(setting.textColor);
                leftNotifyStopWatch.Reset();
            }
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
