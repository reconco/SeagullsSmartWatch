using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeagullsSmartWatch
{
    public class NotifyPatternData
    {
        public const int MAX_COUNT = 16;
        public int Time { get; set; } = 0;
        public string Message { get; set; } = "";
        public string MessageColor { get; set; } = "#FFFFFFFF";
        public string SoundFile { get; set; } = "";

        public NotifyPatternData Clone()
        {
            NotifyPatternData cloned = new NotifyPatternData();
            cloned.Time = this.Time;
            cloned.Message = this.Message;
            cloned.MessageColor = this.MessageColor;
            cloned.SoundFile = this.SoundFile;

            return cloned;
        }
    }
}
