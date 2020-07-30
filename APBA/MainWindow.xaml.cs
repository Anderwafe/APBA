using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Un4seen.Bass;
using Microsoft.Win32;
using System.IO;
using WindowsHook;
using System.Windows.Media;
using System.Threading;

namespace APBA
{
    public partial class MainWindow : Window
    {

        Random random = new Random(DateTime.Now.Millisecond);

        public MainWindow()
        {
            InitializeComponent();

            Dispatcher.InvokeAsync(() => { Timer timer = new Timer(new TimerCallback(CollectGarbage), null, 0, 5000); });
            

            #region Global Hooks

            IKeyboardMouseEvents KeyHook = Hook.GlobalEvents();
            KeyHook.KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case WindowsHook.Keys.MediaNextTrack:
                        BassMet.PlayNext();
                        break;

                    case WindowsHook.Keys.MediaPlayPause:
                        if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_PAUSED)
                            BassMet.Resume();
                        else
                            BassMet.Pause();
                        break;

                    case WindowsHook.Keys.MediaPreviousTrack:
                        BassMet.PlayPrev();
                        break;

                    case WindowsHook.Keys.MediaStop:
                        BassMet.Stop();
                        break;
                }
            };

            #endregion

            BassMet.ggg = this;
            var addons = Bass.BASS_PluginLoadDirectory(Environment.CurrentDirectory);

            #region Buttons logic

            btnShaffle.Click += (e, a) =>
            {
                int j = 0;
                for(int i = BassMet.PlayList.Count-1; i >= 1; i--)
                {
                    j = random.Next(i);
                    var buf = BassMet.PlayList[j];
                    BassMet.PlayList[j] = BassMet.PlayList[i];
                    BassMet.PlayList[i] = buf;
                    if (BassMet.now == i)
                        BassMet.now = j;
                    if (BassMet.now == j)
                        BassMet.now = i;
                }

                UpdateList();
            };

            btnStop.Click += (e, a) =>
            {
                BassMet.Stop();
            };

            btnResumePause.Click += (e, a) =>
            {
                if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_PAUSED)
                    BassMet.Resume();
                else
                    BassMet.Pause();
            };

            btnNext.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 0)
                {
                    BassMet.PlayNext();
                }
            };

            btnPrev.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 0)
                {
                    BassMet.PlayPrev();
                }
            };

            btnLoop.Click += (e, a) =>
            {
                btnLoop.BorderBrush = btnLoop.BorderBrush == Brushes.Red ? Brushes.Blue : Brushes.Red;
            };

            #endregion

            #region ListViews logic

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
                            foreach(var file in Directory.GetFiles(obj))
                            {
                                if (Utils.BASSAddOnIsFileSupported(addons, file))
                                {
                                    BassMet.PlayList.Add(new Playlists { Name = file.Substring(file.LastIndexOfAny(new char[] { '\\', '/' }) + 1), Path = file });
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show("Не удалось загрузить файл '" + file + "'");
                                }
                            }
                        }
                        else
                        {
                            if(Utils.BASSAddOnIsFileSupported(addons, obj))
                            {
                                BassMet.PlayList.Add(new Playlists { Name = obj.Substring(obj.LastIndexOfAny(new char[] { '\\', '/' })+1), Path = obj });
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

            #endregion

            #region Context Main Menu logic

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

            #endregion

            #region Sliders logic

            BassMet.slrVolume = (float)slrPlayVolume.Value;

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

            #endregion

            BassMet.timer.Tick += (e, a) =>
            {
                if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_STOPPED)
                {
                    Brush b = Brushes.Blue;
                    Dispatcher.Invoke(() =>
                    {
                        b = btnLoop.BorderBrush;
                    });
                    if (b == Brushes.Blue)
                        BassMet.Replay();
                    else
                        BassMet.PlayNext();
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
            
        }

        public static void CollectGarbage(object obj)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void SyncSlider()
        {
                slrPlayDuration.Maximum = Bass.BASS_ChannelBytes2Seconds(BassMet._stream, Bass.BASS_ChannelGetLength(BassMet._stream));
                slrPlayDuration.Value = 0;
            
        }

        void lvPlaylistDoUpper(in int index)
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

        void lvPlaylistDoLower(in int index)
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
