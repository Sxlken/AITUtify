using System;

class Program
{
    static void Main(string[] args)
    {
        MusicDatabase database = new MusicDatabase();

        UserInterface ui = new UserInterface(database);
        ui.MainMenu();
    }
}
