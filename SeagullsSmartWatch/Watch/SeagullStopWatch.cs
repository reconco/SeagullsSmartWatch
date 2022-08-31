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

        List<int> notifyTimePattern = new List<int>();
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
            nextNotifyTime += new TimeSpan(MainWindow.setting.useNotify_hours, MainWindow.setting.useNotify_minutes, MainWindow.setting.useNotify_seconds);
        }

        public override void Update()
        {
            if (!MainWindow.setting.useNotify)
                return;

            if (!IsRunning)
                return;

            if ((nextNotifyTime - CurrentTime).TotalSeconds <= 0)
            {
                mainWindow.StartNotification();
                nextNotifyTime += new TimeSpan(MainWindow.setting.useNotify_hours, MainWindow.setting.useNotify_minutes, MainWindow.setting.useNotify_seconds);
            }

            base.Update();
        }

        public void SetNotifyPatternData(List<NotifyPatternData> notifyPatternData)
        {
            notifyTimePattern.Clear();

            foreach (NotifyPatternData patternData in notifyPatternData)
            {
                notifyTimePattern.Add(patternData.Time);
            }

            curNotifyTimeIndex = 0;
            CalculateNextNotifyTime();
        }

        private void CalculateNextNotifyTime()
        {
            TimeSpan ts = new TimeSpan(0, 0, notifyTimePattern[curNotifyTimeIndex]);
            nextNotifyTime = nextNotifyTime.Add(ts);
        }
    }
}
