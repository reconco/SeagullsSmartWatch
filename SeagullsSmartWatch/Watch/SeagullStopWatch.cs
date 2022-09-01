using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeagullsSmartWatch
{
    class SeagullStopWatch : SeagullWatch
    {
        TimeSpan nextNotifyTime = new TimeSpan();

        NotifyPatternData nextNotifyPatternData = null;
        int curNotifyTimeIndex = 0;

        public override TimeSpan CurrentTime
        {
            get
            {

                return stopWatch.Elapsed;
            }
        }

        public SeagullStopWatch(MainWindow _mainWindow) : base(_mainWindow)
        {
            CalculateNextNotifyTime();
        }

        public override void Update()
        {
            if (!MainWindow.setting.useNotify && !MainWindow.setting.useNotifyPattern)
                return;

            if (!IsRunning)
                return;

            if ((nextNotifyTime - CurrentTime).TotalSeconds <= 0)
            {
                if(nextNotifyPatternData == null)
                    mainWindow.StartNotification();
                else
                {
                    mainWindow.StartNotification(/*nextNotifyPatternData.SoundFile,*/ nextNotifyPatternData.NotifyTextColor, nextNotifyPatternData.Message);
                    nextNotifyPatternData = null;
                }
                CalculateNextNotifyTime();
            }

            base.Update();
        }

        public void SetToFirstNotifyPatternIndex()
        {
            curNotifyTimeIndex = 0;
        }

        private void CalculateNextNotifyTime()
        {
            if (MainWindow.setting.useNotify)
            {
                nextNotifyTime += new TimeSpan(MainWindow.setting.useNotify_hours, MainWindow.setting.useNotify_minutes, MainWindow.setting.useNotify_seconds);
            }
            else if (MainWindow.setting.useNotifyPattern)
            {
                nextNotifyPatternData = MainWindow.setting.notifyPatterns[curNotifyTimeIndex];
                TimeSpan ts = new TimeSpan(0, 0, MainWindow.setting.notifyPatterns[curNotifyTimeIndex].Time);
                nextNotifyTime = nextNotifyTime.Add(ts);

                curNotifyTimeIndex++;
                if (curNotifyTimeIndex >= MainWindow.setting.notifyPatterns.Count)
                    curNotifyTimeIndex = 0;
            }
        }
    }
}
