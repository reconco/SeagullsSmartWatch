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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Dsafa.WpfColorPicker;
using Microsoft.Win32;

namespace SeagullsSmartWatch
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Window
    {

        private MainWindow mainWindow = null;
        private NotifyPatternWindow notifyPatternWindow = null;
        public List<NotifyPatternData> tempNotifyPatterns = new List<NotifyPatternData>();
        private bool needReset = false;

        public SettingWindow(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;

            InitializeComponent();

            SetCurrentSetting();

            if (findSoundButton.Content as string == "")
                findSoundButton.Content = "파일 없음";
        }

        private void SetCurrentSetting()
        {
            WatchSetting setting = MainWindow.setting;

            if (setting.watchType == WatchType.Stopwatch)
                stopwatchRadioButton.IsChecked = true;
            else
                timerRadioButton.IsChecked = true;

            hideWatchTypeText.IsChecked = setting.hideWatchTypeText;

            Color _bgColor = (Color)ColorConverter.ConvertFromString(setting.bgColor);
            backgroundColor.Background = new SolidColorBrush(_bgColor);

            Color _textColor = (Color)ColorConverter.ConvertFromString(setting.textColor);
            textColor.Background = new SolidColorBrush(_textColor);

            nofitySoundCheckbox.IsChecked = setting.useNotifySound;
            findSoundButton.Content = setting.notiSoundFile;

            Color _notiTextColor = (Color)ColorConverter.ConvertFromString(setting.notifyTextColor);
            notifyTextColor.Background = new SolidColorBrush(_notiTextColor);
            notifyTextTime.Text = setting.notifyTextTime.ToString();

            useNotificationCheckbox.IsChecked = setting.useNotify;
            notiHour.Text = setting.useNotify_hours.ToString();
            notiMin.Text = setting.useNotify_minutes.ToString();
            notiSec.Text = setting.useNotify_seconds.ToString();
            if (!setting.useNotify)
                useNotificationCheckbox_Unchecked(this, null);

            timerHour.Text = setting.timer_hours.ToString();
            timerMin.Text = setting.timer_minutes.ToString();
            timerSec.Text = setting.timer_seconds.ToString();

            useNotificationPatternCheckbox.IsChecked = setting.useNotifyPattern;
            tempNotifyPatterns.Clear();
            foreach (NotifyPatternData data in MainWindow.setting.notifyPatterns)
                tempNotifyPatterns.Add(data.Clone());
        }

        private void SaveNewSetting()
        {
            WatchSetting newSetting = new WatchSetting();

            if (stopwatchRadioButton.IsChecked == true)
                newSetting.watchType = WatchType.Stopwatch;
            else
                newSetting.watchType = WatchType.Timer;

            newSetting.hideWatchTypeText = (hideWatchTypeText.IsChecked == true);

            SolidColorBrush bgBrush = backgroundColor.Background as SolidColorBrush;
            newSetting.bgColor = bgBrush.Color.ToString();

            SolidColorBrush textBrush = textColor.Background as SolidColorBrush;
            newSetting.textColor = textBrush.Color.ToString();

            newSetting.useNotifySound = (nofitySoundCheckbox.IsChecked == true);
            newSetting.notiSoundFile = findSoundButton.Content as string;

            SolidColorBrush notiTextBrush = notifyTextColor.Background as SolidColorBrush;
            newSetting.notifyTextColor = notiTextBrush.Color.ToString();

            newSetting.notifyTextTime = int.Parse(notifyTextTime.Text);

            newSetting.useNotify = (useNotificationCheckbox.IsChecked == true);
            newSetting.useNotify_hours = int.Parse(notiHour.Text);
            newSetting.useNotify_minutes = int.Parse(notiMin.Text);
            newSetting.useNotify_seconds = int.Parse(notiSec.Text);

            newSetting.timer_hours = int.Parse(timerHour.Text);
            newSetting.timer_minutes = int.Parse(timerMin.Text);
            newSetting.timer_seconds = int.Parse(timerSec.Text);

            newSetting.useNotifyPattern = (useNotificationPatternCheckbox.IsChecked == true);
            newSetting.notifyPatterns = tempNotifyPatterns;

            MainWindow.setting = newSetting;
            MainWindow.SaveSettingData();
        }

        private bool NotifyTimeIsSame()
        {
            WatchSetting curSetting = MainWindow.setting;

            if((useNotificationCheckbox.IsChecked == true) && !curSetting.useNotify)
                return false;
            else if ((useNotificationPatternCheckbox.IsChecked == true) && !curSetting.useNotifyPattern)
                return false;

            else if (useNotificationCheckbox.IsChecked == true)
            {
                if (curSetting.useNotify_hours != int.Parse(notiHour.Text))
                    return false;
                else if (curSetting.useNotify_minutes != int.Parse(notiMin.Text))
                    return false;
                else if (curSetting.useNotify_seconds != int.Parse(notiSec.Text))
                    return false;
            }
            else if(useNotificationPatternCheckbox.IsChecked == true)
            {
                if (notifyPatternSaveMessage.Visibility == Visibility.Visible)
                    return false;
            }

            return true;
        }

        private bool WatchTypeIsSame()
        {
            if ((stopwatchRadioButton.IsChecked == true) && 
                (MainWindow.setting.watchType == WatchType.Stopwatch))
                return true;
            else if ((timerRadioButton.IsChecked == true) &&
                (MainWindow.setting.watchType == WatchType.Timer))
                return true;

            return false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if(notifyPatternWindow != null)
            {
                MessageBox.Show("알림 패턴 설정이 열려 있습니다. 알림 패턴 설정 창을 닫아주세요.");
                return;
            }

            SaveNewSetting();

            if (needReset)
                mainWindow.Reset();

            mainWindow.soundPlayer.LoadSoundFile(findSoundButton.Content as string);
            mainWindow.ChangeWatchTypeText();
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (notifyPatternWindow != null)
            {
                MessageBox.Show("알림 패턴 설정 창이 열려 있습니다. 알림 패턴 설정 창을 닫아주세요.");
                return;
            }

            Close();
        }
        private void useNotificationCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            noti1MinButton.IsEnabled = true;
            noti5MinButton.IsEnabled = true;
            noti10MinButton.IsEnabled = true;
            noti30MinButton.IsEnabled = true;
            noti60MinButton.IsEnabled = true;

            notiHour.IsEnabled = true;
            notiMin.IsEnabled = true;
            notiSec.IsEnabled = true;

            useNotificationPatternCheckbox.IsChecked = false;
        }

        private void useNotificationCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            noti1MinButton.IsEnabled = false;
            noti5MinButton.IsEnabled = false;
            noti10MinButton.IsEnabled = false;
            noti30MinButton.IsEnabled = false;
            noti60MinButton.IsEnabled = false;

            notiHour.IsEnabled = false;
            notiMin.IsEnabled = false;
            notiSec.IsEnabled = false;
        }
        private void useNotificationPattern_Checked(object sender, RoutedEventArgs e)
        {
            useNotificationCheckbox.IsChecked = false;
        }

        private void SetNotifyTime(int hour, int minutes, int seconds)
        {
            notiHour.Text = hour.ToString();
            notiMin.Text = minutes.ToString();
            notiSec.Text = seconds.ToString();
        }

        private void SetTimerTime(int hour, int minutes, int seconds)
        {
            timerHour.Text = hour.ToString();
            timerMin.Text = minutes.ToString();
            timerSec.Text = seconds.ToString();
        }

        private void notifyQuickButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string buttonString = button.Content as string;

            if (buttonString == "1분")
                SetNotifyTime(0, 1, 0);
            else if (buttonString == "5분")
                SetNotifyTime(0, 5, 0);
            else if(buttonString == "10분")
                SetNotifyTime(0, 10, 0);
            else if(buttonString == "30분")
                SetNotifyTime(0, 30, 0);
            else if(buttonString == "1시간")
                SetNotifyTime(1, 0, 0);
        }

        private void timerQuickButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string buttonString = button.Content as string;

            if (buttonString == "5분")
                SetTimerTime(0, 5, 0);
            else if (buttonString == "10분")
                SetTimerTime(0, 10, 0);
            else if (buttonString == "30분")
                SetTimerTime(0, 30, 0);
            else if (buttonString == "1시간")
                SetTimerTime(1, 0, 0);
            else if (buttonString == "2시간")
                SetTimerTime(2, 0, 0);
        }

        private void backgroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush bgSolidBrush = backgroundColor.Background as SolidColorBrush;
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog(bgSolidBrush.Color);

            Color newColor = Colors.White;
            bool? res = colorPickerDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                newColor = colorPickerDialog.Color;
                SolidColorBrush newBrush = new SolidColorBrush(newColor);
                backgroundColor.Background = newBrush;

                mainWindow.SetBGColor(newColor.ToString());
            }
        }

        private void textColorButton_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush textSolidBrush = textColor.Background as SolidColorBrush;
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog(textSolidBrush.Color);

            Color newColor = Colors.White;
            bool? res = colorPickerDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                newColor = colorPickerDialog.Color;
                SolidColorBrush newBrush = new SolidColorBrush(newColor);
                textColor.Background = newBrush;

                mainWindow.SetTextColor(newColor.ToString());
            }
        }

        private void notifyTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush notiTextSolidBrush = textColor.Background as SolidColorBrush;
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog(notiTextSolidBrush.Color);

            Color newColor = Colors.White;
            bool? res = colorPickerDialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                newColor = colorPickerDialog.Color;
                SolidColorBrush newBrush = new SolidColorBrush(newColor);
                notifyTextColor.Background = newBrush;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (notifyPatternWindow != null)
            {
                MessageBox.Show("알림 패턴 설정 창이 열려 있습니다. 알림 패턴 설정 창을 닫아주세요.");
                e.Cancel = true;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainWindow.SetBGColor(MainWindow.setting.bgColor.ToString());
            mainWindow.SetTextColor(MainWindow.setting.textColor.ToString());
        }

        private void findSoundButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog.Filter = "Sound files (*.mp3,*.wav)|*.mp3;*.wav|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                Uri fileUri = new Uri(openFileDialog.FileName);
                Uri referenceUri = new Uri(System.AppDomain.CurrentDomain.BaseDirectory);
                findSoundButton.Content = referenceUri.MakeRelativeUri(fileUri).ToString();
            }
        }

        private void setNotificationPattern_Click(object sender, RoutedEventArgs e)
        {
            if (notifyPatternWindow == null)
            {
                notifyPatternWindow = new NotifyPatternWindow(tempNotifyPatterns);
                notifyPatternWindow.NotifyPatternHasChanged += NotifyPatternWindow_NotifyPatternHasChanged;
                notifyPatternWindow.Closed += NotifyPatternWindow_Closed; ;
                notifyPatternWindow.Show();
            }
            else
            {
                notifyPatternWindow.Focus();
            }
        }

        private void NotifyPatternWindow_NotifyPatternHasChanged(object sender, EventArgs e)
        {
            NotifyPatternWindow notifyPatternWindow = sender as NotifyPatternWindow;
            if (notifyPatternWindow == null)
                return;

            notifyPatternSaveMessage.Visibility = Visibility.Visible;

            tempNotifyPatterns = notifyPatternWindow.CloneNotifyPatternDatas();
        }

        private void NotifyPatternWindow_Closed(object sender, EventArgs e)
        {
            notifyPatternWindow = null;
        }

        private void UpdateGuideText()
        {
            if ((timerRadioButton.IsChecked == true))
            {
                guideText.Text = "* 확인을 누르면 진행 시간이 초기화 됩니다." + Environment.NewLine + "(사유 : 시계 타입 타이머)";
                needReset = true;
            }
            else if (!WatchTypeIsSame())
            {
                guideText.Text = "* 확인을 누르면 진행 시간이 초기화 됩니다." + Environment.NewLine + "(사유 : 시계 타입 변경)";
                needReset = true;
            }
            else if (!NotifyTimeIsSame())
            {
                guideText.Text = "* 확인을 누르면 다음 알림 이후부터 변경된 알림설정이 적용됩니다." + Environment.NewLine + "(사유 : 스톱워치 알림 설정 변경)";
                needReset = false;
            }
            else
            {
                guideText.Text = ""; 
                needReset = false;
            }
        }

        private void Window_LayoutUpdated(object sender, EventArgs e)
        {
            UpdateGuideText();
        }
    }
}
