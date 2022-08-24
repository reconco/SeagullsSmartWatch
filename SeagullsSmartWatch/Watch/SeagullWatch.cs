using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace SeagullsSmartWatch
{
    abstract class SeagullWatch
    {
        protected Stopwatch stopWatch = new Stopwatch();
        protected MainWindow mainWindow = null;

        public bool IsRunning { get { return stopWatch.IsRunning; } }

        public SeagullWatch(MainWindow _mainWindow)
        {
            this.mainWindow = _mainWindow;
        }

        public virtual TimeSpan CurrentTime
        {
            get
            {
                return stopWatch.Elapsed;
            }
        }


        public virtual void Update()
        {
        }

        public void Start()
        {
            stopWatch.Start();
        }

        public void Stop()
        {
            stopWatch.Stop();
        }

        public void Reset()
        {
            stopWatch.Reset();
        }
    }
}
