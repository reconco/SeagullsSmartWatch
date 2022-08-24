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

        private void StartCommonNotification()
        {
            SetTextColor(MainWindow.setting.notifyTextColor);
            leftNotifyingStopWatch.Start();
        }

        public void StartNotification()
        {
            StartCommonNotification();

            if (MainWindow.setting.useNotifySound)
                soundPlayer.Play();
        }

        public void StartNotification(string soundFile)
        {
            StartCommonNotification();

            if (MainWindow.setting.useNotifySound)
            {
                soundPlayer.LoadSoundFile(soundFile);
                soundPlayer.Play();
            }
        }

        private void UpdateNotification(TimeSpan currentTime)
        {
            if (leftNotifyingStopWatch.Elapsed.TotalSeconds >= setting.notifyTextTime)
            {
                SetTextColor(setting.textColor);
                leftNotifyingStopWatch.Reset();
            }
        }
    }
}
