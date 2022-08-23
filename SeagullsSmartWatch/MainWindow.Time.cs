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
        Stopwatch stopWatch = new Stopwatch();
        public TimeSpan TimerTargetTime { get; set; } = new TimeSpan();
        TimeSpan nextNotifyTime = new TimeSpan();
        Stopwatch leftNotifyStopWatch = new Stopwatch();

        TimeSpan previousResetTime = new TimeSpan();

        public TimeSpan NextNotifyTime { get { return nextNotifyTime; } }
        public bool WatchHadStarted { get { return (stopWatch.ElapsedMilliseconds > 0); } }


        private void updateNotifycation(TimeSpan currentTime)
        {
            if (setting.watchType == WatchType.Stopwatch &&
                stopWatch.IsRunning)
            {
                if ((nextNotifyTime - currentTime).TotalSeconds <= 0)
                {
                    if (setting.useNotify)
                    {
                        SetTextColor(setting.notifyTextColor);
                        leftNotifyStopWatch.Start();
                        nextNotifyTime += new TimeSpan(setting.useNotify_hours, setting.useNotify_minutes, setting.useNotify_seconds);

                        if (setting.useNotifySound)
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

            if (leftNotifyStopWatch.Elapsed.TotalSeconds >= setting.notifyTextTime)
            {
                SetTextColor(setting.textColor);
                leftNotifyStopWatch.Reset();
            }
        }
    }
}
