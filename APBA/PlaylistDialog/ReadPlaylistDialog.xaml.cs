using PlaylistsNET.Content;
using PlaylistsNET.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace APBA
{
    /// <summary>
    /// Логика взаимодействия для ReadPlaylistDialog.xaml
    /// </summary>
    public partial class ReadPlaylistDialog : Window
    {
        List<string> LPlaylists = new List<string>();
        public ReadPlaylistDialog()
        {
            InitializeComponent();

            Activated += (e, a) =>
            {
                try
                {
                    var b = Directory.GetFiles(Playlists.PLPath, "*.m3u");
                    if (b == null)
                    {
                        MessageBox.Show("Плейлисты не найдены", "Ошибка", MessageBoxButton.OK);
                    }
                    else
                    {
                        for (int i = 0; i < b.Length; i++)
                        {
                            
                            LPlaylists.Add(b[i].Substring(b[i].LastIndexOfAny(new char[] { '\\', '/' }) + 1));
                        }

                        //lvOpenPlaylist.DisplayMemberPath = LPlaylists[0].ToString();
                        lvOpenPlaylist.ItemsSource = LPlaylists;
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("Плейлисты не найдены", "Ошибка", MessageBoxButton.OK);
                    Directory.CreateDirectory(Playlists.PLPath);
                }
            };

            lvOpenPlaylist.MouseDoubleClick += (e, a) =>
            {
                try
                {
                    if (LPlaylists.Count == 0)
                        return;
                    Playlists.PlayListRead(LPlaylists[lvOpenPlaylist.SelectedIndex]);
                    Close();
                }
                catch
                { }
            };
        }
    }
}
