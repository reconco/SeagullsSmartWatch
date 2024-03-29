﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeagullsSmartWatch
{
    public partial class MainWindow
    {
        public static readonly string SAVE_FILE_PATH = "setting.save";
        public static SaveFile saveFile = new SaveFile(SAVE_FILE_PATH);

        private void LoadSaveData()
        {
            if (saveFile.OpenSaveFile())
            {
                setting.watchType = (WatchType)saveFile.GetData("WatchType", 0);

                setting.hideWatchTypeText = (bool)saveFile.GetData("ShowWatchType", false);
                setting.bgColor = (string)saveFile.GetData("bgColor", "#FFFFFFFF");
                setting.textColor = (string)saveFile.GetData("textColor", "#FF000000");

                setting.useNotifySound = (bool)saveFile.GetData("useNotifySound", false);
                setting.notiSoundFile = (string)saveFile.GetData("notifySoundFile", "Sounds\\BeepBeepBeep.wav");

                setting.notifyTextColor = (string)saveFile.GetData("notifyTextColor", "#FFFF0000");
                setting.notifyTextTime = (int)saveFile.GetData("NotifyTextTime", 3);

                setting.useNotify = (bool)saveFile.GetData("UseNotify", false);
                setting.useNotify_hours = (int)saveFile.GetData("UseNotify_H", 0);
                setting.useNotify_minutes = (int)saveFile.GetData("UseNotify_M", 0);
                setting.useNotify_seconds = (int)saveFile.GetData("UseNotify_S", 0);

                setting.timer_hours = (int)saveFile.GetData("Timer_H", 0);
                setting.timer_minutes = (int)saveFile.GetData("Timer_M", 0);
                setting.timer_seconds = (int)saveFile.GetData("Timer_S", 0);

                setting.useNotifyPattern = (bool)saveFile.GetData("UseNotifyPattern", false);

                setting.notifyPatterns.Clear();
                int notifyPatternCount = (int)saveFile.GetData("NotifyPatternCount", 0);
                for(int i = 0; i < notifyPatternCount; i++)
                {
                    NotifyPatternData notifyPatternData = new NotifyPatternData();
                    string dataName = "NotifyPattern" + i.ToString();

                    notifyPatternData.Time = (int)saveFile.GetData(dataName + "_Time", 0);
                    notifyPatternData.Message = (string)saveFile.GetData(dataName + "_Message", "");
                    notifyPatternData.NotifyTextColor = (string)saveFile.GetData(dataName + "_NotifyTextColor", "");

                    setting.notifyPatterns.Add(notifyPatternData);
                }
            }
            else
                SaveSettingData();

            //if (setting.watchType == WatchType.Timer)
            //{
            //    int h = setting.timer_hours;
            //    int m = setting.timer_minutes;
            //    int s = setting.timer_seconds;
            //    TimerTargetTime = new TimeSpan(h, m, s);
            //    updateTick(this, null);
            //}

        }

        public static void SaveSettingData()
        {
            saveFile.SetData("WatchType", (int)setting.watchType);
            saveFile.SetData("ShowWatchType", setting.hideWatchTypeText);
            saveFile.SetData("bgColor", setting.bgColor);
            saveFile.SetData("textColor", setting.textColor);

            saveFile.SetData("useNotifySound", setting.useNotifySound);
            saveFile.SetData("notifySoundFile", setting.notiSoundFile);

            saveFile.SetData("notifyTextColor", setting.notifyTextColor);
            saveFile.SetData("NotifyTextTime", setting.notifyTextTime);

            saveFile.SetData("UseNotify", setting.useNotify);
            saveFile.SetData("UseNotify_H", setting.useNotify_hours);
            saveFile.SetData("UseNotify_M", setting.useNotify_minutes);
            saveFile.SetData("UseNotify_S", setting.useNotify_seconds);

            saveFile.SetData("Timer_H", setting.timer_hours);
            saveFile.SetData("Timer_M", setting.timer_minutes);
            saveFile.SetData("Timer_S", setting.timer_seconds);

            saveFile.SetData("UseNotifyPattern", setting.useNotifyPattern);

            int notifyPatternCount = setting.notifyPatterns.Count;
            saveFile.SetData("NotifyPatternCount", setting.notifyPatterns.Count);
            for(int i =0; i < notifyPatternCount; i++)
            {
                string dataName = "NotifyPattern" + i.ToString();
                saveFile.SetData(dataName + "_Time", setting.notifyPatterns[i].Time);
                saveFile.SetData(dataName + "_Message", setting.notifyPatterns[i].Message);
                saveFile.SetData(dataName + "_NotifyTextColor", setting.notifyPatterns[i].NotifyTextColor);
            }

            saveFile.SaveThisFile();
        }
    }
}
