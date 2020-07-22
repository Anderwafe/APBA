using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для SavePlaylist.xaml
    /// </summary>
    public partial class SavePlaylistDialog : Window
    {
        public SavePlaylistDialog()
        {
            InitializeComponent();
            SavePlaylistOK.Click += (e, a) =>
            {
                Dispatcher.InvokeAsync(() => {
                    if (Playlists.PlayListSave(SavePlaylistName.Text)) 
                    {
                        DialogResult = true;
                        Close();
                    } 
                    else 
                    {
                        MessageBox.Show("Плейлист с таким названием уже существует!", "Ошибка", MessageBoxButton.OK);
                    } 
                });
            };
        }
    }
}
