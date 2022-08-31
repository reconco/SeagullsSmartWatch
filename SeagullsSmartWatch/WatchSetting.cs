using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeagullsSmartWatch
{
    public enum WatchType
    {
        Stopwatch, Timer
    }

    public class WatchSetting
    {
        public WatchType watchType = WatchType.Stopwatch;

        public bool hideWatchTypeText = false;
        public string bgColor = "#FFFFFFFF";
        public string textColor = "#FF000000";

        public bool useNotifySound = false;
        public string notiSoundFile = "Sounds\\BeepBeepBeep.wav";
        public string notifyTextColor = "#FFFF0000";
        public int notifyTextTime = 0;

        public bool useNotify = false;
        public int useNotify_hours = 0;
        public int useNotify_minutes = 0;
        public int useNotify_seconds = 0;

        public int timer_hours = 0;
        public int timer_minutes = 0;
        public int timer_seconds = 0;

        public bool useNotifyPattern = false;
        public List<NotifyPatternData> notifyPatterns = new List<NotifyPatternData>();
    }


}
