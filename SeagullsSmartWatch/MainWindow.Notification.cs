using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace SeagullsSmartWatch
{

    public partial class MainWindow
    {
        Stopwatch leftNotifyingStopWatch = new Stopwatch();
        public bool usingDayTextToMessage = false;

        private void StartCommonNotification()
        {
            SetTextColor(MainWindow.setting.notifyTextColor);
            leftNotifyingStopWatch.Reset();
            leftNotifyingStopWatch.Start();
        }

        public void StartNotification()
        {
            StartCommonNotification();

            if (MainWindow.setting.useNotifySound)
                soundPlayer.Play();
        }

        //public void StartNotification(string soundFile)
        //{
        //    StartCommonNotification();

        //    if (MainWindow.setting.useNotifySound)
        //    {
        //        //미구현
        //        //soundPlayer.LoadSoundFile(soundFile);
        //        soundPlayer.Play();
        //    }
        //}
        public void StartNotification(/*string soundFile,*/ string notifyTextColor, string message)
        {
            SetTextColor(notifyTextColor);
            leftNotifyingStopWatch.Reset();
            leftNotifyingStopWatch.Start();

            if (MainWindow.setting.useNotifySound)
            {
                //미구현
                //soundPlayer.LoadSoundFile(soundFile);
                soundPlayer.Play();
            }

            if (message != "")
            {
                day.Text = message;
                day.Visibility = Visibility.Visible;
                usingDayTextToMessage = true;
            }
        }


        private void UpdateNotification(TimeSpan currentTime)
        {
            if (leftNotifyingStopWatch.Elapsed.TotalSeconds >= setting.notifyTextTime)
            {
                SetTextColor(setting.textColor);
                leftNotifyingStopWatch.Reset();
                usingDayTextToMessage = false;
            }
        }
    }
}
