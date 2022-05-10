using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace SeagullsSmartWatch
{
    enum ABC
    {
        aaa, bbb,
    }

    public class SoundPlayer
    {
        bool opened = false;
        MediaPlayer mediaPlayer = new MediaPlayer();

        enum ABCD
        {
            aaad, bbbd,
        }

        public void LoadSoundFile(string path)
        {
            if (!path.Contains(System.AppDomain.CurrentDomain.BaseDirectory))
                path = System.AppDomain.CurrentDomain.BaseDirectory + path;

            try
            {
                mediaPlayer.Volume = 0;             //사운드 파일 로드할때 잠깐 소리가 나는 버그가 있어서 임시로 볼륨을 없애버림
                mediaPlayer.Open(new Uri(path));
                opened = true;
            }
            catch
            {
                MessageBox.Show("사운드 파일 로드 실패 : " + path);
            }
        }

        public void Play()
        {
            if (!opened)
                return;

            if (MainWindow.isMute)
                return;

            mediaPlayer.Volume = 0.5;
            mediaPlayer.Stop();
            mediaPlayer.Play();
        }
        public void Pause()
        {
            if (!opened)
                return;

            mediaPlayer.Pause();
        }

        public void Stop()
        {
            if (!opened)
                return;

            mediaPlayer.Stop();
        }
    }
}
