using System;
using System.Collections.Generic;
public class MusicDatabase
{
    private List<Song> songs = new List<Song>();

    public void AddSong(Song song)
    {
        songs.Add(song);
    }

    public List<Song> SearchByTitle(string title)
    {
        return songs.FindAll(song => song.Title.ToLower().Contains(title.ToLower()));
    }

}
    