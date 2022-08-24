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
    }
}
