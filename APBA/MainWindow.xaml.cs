using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Un4seen.Bass;
using Microsoft.Win32;
using System.IO;

namespace APBA
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            var addons = Bass.BASS_PluginLoadDirectory(Environment.CurrentDirectory);
            Bass.BASS_Init(-1, BassMet.hz, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            #region
            /*btnPlay.Click += (e, a) => 
            {
                OpenFileDialog OFD = new OpenFileDialog();
                    OFD.Multiselect = true;
                if (OFD.ShowDialog() == true)
                {
                    for (int i = 0; i < OFD.FileNames.Length; i++)
                    {
                        if (Utils.BASSAddOnIsFileSupported(addons, OFD.FileNames[i]))
                        {
                            int stream = Bass.BASS_StreamCreateFile(OFD.FileNames[i], 0, 0, BASSFlag.BASS_DEFAULT);
                            BassMet.PlayList.Add(new Playlists
                            {
                                Name = OFD.SafeFileNames[i],
                                Path = OFD.FileNames[i],
                                Duration = Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream))
                            });
                            Bass.BASS_StreamFree(stream);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось загрузить файл '" + OFD.FileNames[i] + "'");
                        }
                    }
                    UpdateList();
                }
            };*/
            #endregion

            btnStop.Click += (e, a) =>
            {
                BassMet.timer.Stop();
                BassMet.Stop();
            };
            btnPause.Click += (e, a) =>
            {
                BassMet.timer.Stop();
                BassMet.Pause();
            };

            btnResume.Click += (e, a) =>
            {
                BassMet.Resume();
                BassMet.timer.Start();
            };

            btnNext.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 0)
                {
                    BassMet.Next();
                    Dispatcher.Invoke(() => SyncSlider());
                }
            };

            btnPrev.Click += (e, a) =>
            {
                if (BassMet.PlayList.Count > 0)
                {
                    BassMet.Prev();
                    Dispatcher.Invoke(() => SyncSlider());
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
                Dispatcher.Invoke(() => SyncSlider());
                /*UpdateList();*/
            };

            lvPlaylist.DragEnter += (e, a) =>
            {
                if (a.Data.GetDataPresent(DataFormats.FileDrop))
                    a.Effects = DragDropEffects.Copy;
            };

            lvPlaylist.Drop += (e, a) =>
            {
                if (a.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    foreach (string obj in (string[])a.Data.GetData(DataFormats.FileDrop))
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
                                MessageBox.Show("Не удалось загрузить файл '" + obj + "'");
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
            };

            BassMet.timer.Tick += (e, a) =>
            {
                if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_STOPPED)
                {
                    BassMet.Next();
                    Dispatcher.Invoke(() => SyncSlider());
                }
                if (Bass.BASS_ChannelIsActive(BassMet._stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Dispatcher.Invoke(() => slrPlayDuration.Value = BassMet.GetAudioPosition());
                    
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

            /*MainMenuSave.Click += (e,a) =>
            {
                string filename = "";

                using (StreamWriter SW = new StreamWriter(Environment.CurrentDirectory + @"/Playlists/" + filename, false))
                {

                }
            };*/
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
