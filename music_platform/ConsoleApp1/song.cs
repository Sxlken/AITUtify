using System;

public class Song
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Genre { get; set; }
    public int Year { get; set; }

    public override string ToString()
    {
        return $"{Title} by {Artist} ({Year}) - {Genre}";
    }
}
