using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

using Un4seen.Bass;

namespace APBA
{
    public class Playlists
    {
        public string Name { get; set; }
        public string Path;

        public static string PLPath = Environment.CurrentDirectory + @"\Playlists\";

        public static void PlayListRead(string name)
        {
            M3uContent content = new M3uContent();
            M3uPlaylist playlist = content.GetFromString(File.ReadAllText(PLPath + name));
            BassMet.PlayList.Clear();
            foreach(var i in playlist.PlaylistEntries)
            {
                BassMet.PlayList.Add(new Playlists { Name = i.Title, Path = i.Path });
            }
        }

        public static bool PlayListSave(string name)
        {
            M3uPlaylist playlist = new M3uPlaylist();
            playlist.IsExtended = true;
            foreach (var play in BassMet.PlayList)
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
                return false;
            }
            File.WriteAllText(PLPath + name + @".m3u", content.ToText(playlist));
            MessageBox.Show("Плейлист " + name + " успешно сохранен", "Сохранено", MessageBoxButton.OK);
            return true;
        }
    }
}
