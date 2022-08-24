using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeagullsSmartWatch
{
    class SeagullTimer : SeagullWatch
    {

        public SeagullTimer(MainWindow _mainWindow) : base(_mainWindow)
        {
        }
        public SeagullTimer(MainWindow _mainWindow, TimeSpan _timerTargetTime) : base(_mainWindow)
        {
            TimerTargetTime = _timerTargetTime;
        }

        public TimeSpan TimerTargetTime { get; set; } = new TimeSpan();
        public override TimeSpan CurrentTime
        {
            get
            {
                return TimerTargetTime - stopWatch.Elapsed;
            }
        }

        public override void Update()
        {
            if (!IsRunning)
                return;

            if (CurrentTime.TotalSeconds <= 0)
            {
                mainWindow.StartNotification();
                stopWatch.Stop();
            }

            base.Update();
        }
    }
}
