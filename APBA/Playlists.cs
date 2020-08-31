using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using Un4seen.Bass;

namespace APBA
{
    public class Playlists
    {
        public string Name { get; set; }
        public string Path;

        public static string PLPath = Environment.CurrentDirectory + @"\Playlists\";

        public static void PlayListRead(in string name)
        {
            M3uContent content = new M3uContent();
            M3uPlaylist playlist = content.GetFromString(File.ReadAllText(PLPath + name));
            BassMet.PlayList.Clear();
            foreach(var i in playlist.PlaylistEntries)
            {
                BassMet.PlayList.Add(new Playlists { Name = i.Title, Path = i.Path });
            }
        }

        public static bool PlayListSave(in string name, in ObservableCollection<Playlists> Playlist)
        {
            M3uPlaylist playlist = new M3uPlaylist();
            playlist.IsExtended = true;
            foreach (var play in Playlist)
            {
                playlist.PlaylistEntries.Add(new M3uPlaylistEntry
                {
                    Title = play.Name,
                    Album = name,
                    Path = play.Path,
                });
            }
            M3uContent content = new M3uContent();
            if (!Directory.Exists(PLPath))
                Directory.CreateDirectory(PLPath);
            if(File.Exists(PLPath + name +".m3u"))
            {
                if(MessageBox.Show("Перезаписать плейлист?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    return false;
            }
            File.WriteAllText(PLPath + name + @".m3u", content.ToText(playlist));
            MessageBox.Show("Плейлист " + name + " успешно сохранен", "Сохранено", MessageBoxButton.OK);
            return true;
        }
    }
}