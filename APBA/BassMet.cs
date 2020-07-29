using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Un4seen.Bass;

namespace APBA
{
    static class BassMet
    {
        #region Class fields

        public static int hz = 44100;
        public static int now;
        public static int _stream = 0;
        public static BASSTimer timer = new BASSTimer(500);
        static public List<Playlists> PlayList = new List<Playlists>();
        public static float slrVolume;
        static public MainWindow ggg;

        #endregion

        public static void Play(in Playlists PlayItem)
        {
            if (_stream != 0)
                if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Stop();
                }
            _stream = Bass.BASS_StreamCreateFile(PlayItem.Path, 0, 0, BASSFlag.BASS_DEFAULT);
            now = PlayList.IndexOf(PlayItem);
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, slrVolume / 100f);

            ggg.Dispatcher.InvokeAsync(() =>
            {
                ggg.SyncSlider();
                ggg.lblMusicDuration.Content = new TimeSpan(0, 0, (int)Bass.BASS_ChannelBytes2Seconds(BassMet._stream, Bass.BASS_ChannelGetLength(BassMet._stream)));
            });

            Bass.BASS_ChannelPlay(_stream, true);
            timer.Start();
        }

        public static void Replay()
        {
            Bass.BASS_ChannelPlay(_stream, true);
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
            timer.Enabled = false;
            Bass.BASS_ChannelPause(_stream);
        }

        public static void Resume()
        {
            if (_stream == 0 || Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_STOPPED)
            {
                return;
            }
            Bass.BASS_ChannelPlay(_stream, false);
            timer.Enabled = true;
        }

        public static void Stop()
        {
            timer.Enabled = false;
            if(_stream == 0)
            {
                return;
            }
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_ChannelStop(_stream);
            ggg.lblMusicDuration.Content = new TimeSpan();
            ggg.lblDurationNow.Content = new TimeSpan();
        }

        public static void PlayNext()
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

        public static void PlayPrev()
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
