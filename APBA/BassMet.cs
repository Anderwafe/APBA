using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
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
        //static public List<Playlists> PlayList = new List<Playlists>();
        static public ObservableCollection<Playlists> PlayList = new ObservableCollection<Playlists>();
        public static float slrVolume;
        static public MainWindow ggg;

        #endregion

        public static void Play(Playlists PlayItem)
        {
            if(Bass.BASS_GetInfo() == null)
            {
                Bass.BASS_Init(-1, hz, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            }
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_ChannelStop(_stream);

            _stream = Bass.BASS_StreamCreateFile(PlayItem.Path, 0, 0, BASSFlag.BASS_DEFAULT);
            now = PlayList.IndexOf(PlayItem);
            Bass.BASS_ChannelSetAttribute(_stream, BASSAttribute.BASS_ATTRIB_VOL, slrVolume / 100f);

            EqualizerSettings.SetFX(_stream);

            ggg.Dispatcher.Invoke(() =>
            {
                ggg.lblAudioName.Content = PlayItem.Name;
                ggg.SyncSlider();
                ggg.lblMusicDuration.Content = new TimeSpan(0, 0, (int)Bass.BASS_ChannelBytes2Seconds(BassMet._stream, Bass.BASS_ChannelGetLength(BassMet._stream)));
            ggg.btnResumePause.Content = "Pause";
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
            timer.Enabled = false;
            Bass.BASS_ChannelPause(_stream);
        }

        public static void Resume()
        {
            Bass.BASS_ChannelPlay(_stream, false);
            timer.Enabled = true;
        }

        public static void Stop()
        {
            timer.Enabled = false;
            Bass.BASS_StreamFree(_stream);
            Bass.BASS_ChannelStop(_stream);
            ggg.lblAudioName.Content = "";
            ggg.lblMusicDuration.Content = new TimeSpan(0, 0, 0);
        }

        public static void PlayNext()
        {
            if (PlayList.Count > 0)
            {
                if (now + 1 < PlayList.Count())
                {
                    Play(PlayList[now + 1]);
                }
                else
                {
                    Play(PlayList.First());
                }
            }
            else
                Stop();
        }

        public static void PlayPrev()
        {
            if (PlayList.Count > 0)
            {
                if (now - 1 >= 0)
                {
                    Play(PlayList[now - 1]);
                }
                else
                {
                    Play(PlayList.Last());
                }
            }
            else
                Stop();
        }
    }
}
