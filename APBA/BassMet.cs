using System.Collections.Generic;
using System.Linq;
using Un4seen.Bass;

namespace APBA
{
    static class BassMet
    {
        public static int hz = 44100;
        public static int now;
        public static int _stream = 0;
        public static BASSTimer timer = new BASSTimer(500);
        static public List<Playlists> PlayList = new List<Playlists>();


        public static bool Play(Playlists PlayItem)
        {
            timer.Start();
            if (_stream != 0)
                if(Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.BASS_ChannelStop(_stream);
                    Bass.BASS_StreamFree(_stream);
                }
            _stream = Bass.BASS_StreamCreateFile(PlayItem.Path, 0, 0, BASSFlag.BASS_DEFAULT);
            now = PlayList.IndexOf(PlayItem);
            //now = PlayItem;
            return Bass.BASS_ChannelPlay(_stream, true);
            
        }

        public static double GetAudioPosition()
        {
            return Bass.BASS_ChannelBytes2Seconds(_stream, Bass.BASS_ChannelGetPosition(_stream));
        }

        public static void Pause()
        {
            if(_stream == 0 || Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_STOPPED)
            {
                return;
            }
            Bass.BASS_ChannelPause(_stream);
        }

        public static void Resume()
        {
            if (_stream == 0 || Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_STOPPED)
            {
                return;
            }
            Bass.BASS_ChannelPlay(_stream, false);
        }

        public static void Stop()
        {
            timer.Stop();
            if(_stream == 0)
            {
                return;
            }
            Bass.BASS_ChannelStop(_stream);
            Bass.BASS_StreamFree(_stream);
        }

        public static void Next()
        {
            if (PlayList.Count > 0)
            {
                if (now + 1 < PlayList.Count())
                    Play(PlayList[now + 1]);
                else
                    Play(PlayList.First());
            }
            else
                Stop();
        }

        public static void Prev()
        {
            if (PlayList.Count > 0)
            {
                if (now - 1 >= 0)
                    Play(PlayList[now - 1]);
                else
                    Play(PlayList.Last());
            }
            else
                Stop();
        }
    }
}
