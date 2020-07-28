using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Un4seen.Bass;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Windows.Controls;
using WindowsHook;

namespace APBA
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            #region Global Hooks

            IKeyboardMouseEvents KeyHook = Hook.GlobalEvents();
            KeyHook.KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case WindowsHook.Keys.MediaNextTrack:
                        BassMet.Next();
                        break;

                    case WindowsHook.Keys.MediaPlayPause:
                        if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_PAUSED)
                            BassMet.Resume();
                        else
                            BassMet.Pause();
                        break;

                    case WindowsHook.Keys.MediaPreviousTrack:
                        BassMet.Prev();
                        break;

                    case WindowsHook.Keys.MediaStop:
                        BassMet.Stop();
                        break;
                }
            };

            #endregion

            BassMet.ggg = this;
            var addons = Bass.BASS_PluginLoadDirectory(Environment.CurrentDirectory);
            Bass.BASS_Init(-1, BassMet.hz, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            BassMet.slrVolume = (float)slrPlayVolume.Value;

            btnStop.Click += (e, a) =>
            {
                BassMet.Stop();
            };
            btnPause.Click += (e, a) =>
            {
                BassMet.Pause();
            };

            btnResume.Click += (e, a) =>
            {
                BassMet.Resume();
            };

            btnNext.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 0)
                {
                    BassMet.Next();
                }
            };

            btnPrev.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 0)
                {
                    BassMet.Prev();
                }
            };

            lvContextUpper.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 1)
                {
                    lvPlaylistDoUpper(lvPlaylist.SelectedIndex);
                }
                UpdateList();
            };

            lvContextLower.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 1)
                {
                    lvPlaylistDoLower(lvPlaylist.SelectedIndex);
                }
                UpdateList();
            };
            
            lvPlaylist.MouseDoubleClick += (e, a) =>
            {
                if (BassMet.PlayList.Count == 0)
                    return;
                BassMet.Play(BassMet.PlayList[lvPlaylist.SelectedIndex]);
            };

            lvPlaylist.DragEnter += (e, a) =>
            {
                if (a.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                    a.Effects = System.Windows.DragDropEffects.Copy;
            };

            lvPlaylist.Drop += (e, a) =>
            {
                if (a.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                {
                    foreach (string obj in (string[])a.Data.GetData(System.Windows.DataFormats.FileDrop))
                    {
                        if (Directory.Exists(obj))
                        {
                            
                        }
                        else
                        {
                            if(Utils.BASSAddOnIsFileSupported(addons, obj))
                            {
                                int stream = Bass.BASS_StreamCreateFile(obj, 0, 0, BASSFlag.BASS_DEFAULT);
                                BassMet.PlayList.Add(new Playlists { Name = obj.Substring(obj.LastIndexOfAny(new char[] { '\\', '/' })+1), Path = obj });
                                Bass.BASS_StreamFree(stream);
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Не удалось загрузить файл '" + obj + "'");
                            }
                        }
                    }
                    UpdateList();
                }
            };

            MainMenuPLOpen.Click += (e, a) =>
            {
                ReadPlaylistDialog RPD = new ReadPlaylistDialog();
                RPD.ShowInTaskbar = false;
                RPD.ShowDialog();
                UpdateList();
            };

            MainMenuPLSave.Click += (e, a) =>
            {
                SavePlaylistDialog SPD = new SavePlaylistDialog();
                SPD.ShowInTaskbar = false;
                SPD.ShowDialog();

            };

            slrPlayDuration.ValueChanged += (e, a) =>
            {
                if(Mouse.LeftButton == MouseButtonState.Pressed && slrPlayDuration.IsMouseOver)
                    Dispatcher.Invoke(() => Bass.BASS_ChannelSetPosition(BassMet._stream, a.NewValue));
                lblDurationNow.Content = new TimeSpan(0, 0, (int)BassMet.GetAudioPosition());
            };

            slrPlayVolume.ValueChanged += (e, a) =>
            {
                BassMet.slrVolume = (float)a.NewValue;
                Bass.BASS_ChannelSetAttribute(BassMet._stream, BASSAttribute.BASS_ATTRIB_VOL, (float)a.NewValue / 100f);
            };

            BassMet.timer.Tick += (e, a) =>
            {
                if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_STOPPED)
                {
                    BassMet.Next();
                }
                if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Dispatcher.Invoke(() =>
                    {
                        slrPlayDuration.Value = BassMet.GetAudioPosition();
                        lblMusicDuration.Content = new TimeSpan(0, 0, (int)Bass.BASS_ChannelBytes2Seconds(BassMet._stream, Bass.BASS_ChannelGetLength(BassMet._stream)));
                    });
                }
            };

            lvContextDelete.Click += (e, a) =>
            {
                try
                {
                    BassMet.PlayList.RemoveAt(lvPlaylist.SelectedIndex);
                    UpdateList();
                }
                catch
                {

                }
            };
        }

        public void SyncSlider()
        {
                slrPlayDuration.Maximum = Bass.BASS_ChannelBytes2Seconds(BassMet._stream, Bass.BASS_ChannelGetLength(BassMet._stream));
                slrPlayDuration.Value = 0;
            
        }

        void lvPlaylistDoUpper(int index)
        {
            var c = BassMet.PlayList[BassMet.now];
            if (index == 0)
            {
                var b = BassMet.PlayList.First();
                BassMet.PlayList[0] = BassMet.PlayList.Last();
                BassMet.PlayList[BassMet.PlayList.Count - 1] = b;
            }
            else
            {
                var b = BassMet.PlayList[index];
                BassMet.PlayList[index] = BassMet.PlayList[index - 1];
                BassMet.PlayList[index - 1] = b;
            }
            BassMet.now = BassMet.PlayList.IndexOf(c);
        }

        void lvPlaylistDoLower(int index)
        {
            var c = BassMet.PlayList[BassMet.now];
            if (index == BassMet.PlayList.Count - 1)
            {
                var b = BassMet.PlayList.First();
                BassMet.PlayList[0] = BassMet.PlayList.Last();
                BassMet.PlayList[BassMet.PlayList.Count - 1] = b;
            }
            else
            {
                var b = BassMet.PlayList[index];
                BassMet.PlayList[index] = BassMet.PlayList[index + 1];
                BassMet.PlayList[index + 1] = b;
            }
            BassMet.now = BassMet.PlayList.IndexOf(c);
        }

        public void UpdateList()
        {
            lvPlaylist.ItemsSource = null;
            lvPlaylist.ItemsSource = BassMet.PlayList;
        }
    }
}
