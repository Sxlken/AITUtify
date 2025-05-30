﻿using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using NAudio.Wave;

public class UserInterface
{
    private MusicDatabase database;
    private string loggedInUser;
    private const string PasswordsFilePath = "passwords.txt"; // RU: пароли сохраняются в music_platform\ConsoleApp1\bin\Debug\net8.0 // ENG: passwords are saved in music_platform\ConsoleApp1\bin\Debug\net8.0
    private readonly WaveOutEvent waveOut = new WaveOutEvent();

    public UserInterface(MusicDatabase db)
    {
        database = db;
    }

    public void MainMenu()
    {
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("┌──────────────────────────────┐");
            Console.WriteLine("│         AITUtify             │"); Console.Write($"│Logged in as: {(string.IsNullOrEmpty(loggedInUser) ? "You are not logged in" : $"{loggedInUser}")}\n");
            Console.WriteLine("├──────────────────────────────┤");
            Console.WriteLine("│ 1. Login                     │");
            Console.WriteLine("│ 2. Register                  │");
            Console.WriteLine("│ 3. Search for songs          │");
            Console.WriteLine("│ 4. Logout                    │");
            Console.WriteLine("│ 5. Exit                      │");
            Console.WriteLine("└──────────────────────────────┘");
            Console.Write("Enter your choice: ");

            int choice;
            if (!int.TryParse(Console.ReadLine(), out choice))
            {
                Console.WriteLine("Invalid choice. Please try again.");
                Console.WriteLine("Press Enter to continue.");
                Console.ReadLine();
                Console.Clear();
                continue;
            }

            switch (choice)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    Register();
                    break;
                case 3:
                    if (IsLoggedIn())
                        SearchSongs();
                    else
                        Console.WriteLine("Please login first.");
                    Console.WriteLine("Press Enter to continue.");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                case 4:
                    if (IsLoggedIn())
                        Logout();
                    else
                    {
                        Console.WriteLine("You are not logged in.");
                        Console.WriteLine("1. Login");
                        Console.WriteLine("2. Register");
                        Console.Write("Enter your choice: ");
                        int loginOrRegisterChoice;
                        if (!int.TryParse(Console.ReadLine(), out loginOrRegisterChoice))
                        {
                            Console.WriteLine("Invalid choice. Please try again.");
                            Console.WriteLine("Press Enter to continue.");
                            Console.ReadLine();
                            Console.Clear();
                            continue;
                        }

                        switch (loginOrRegisterChoice)
                        {
                            case 1:
                                Login();
                                break;
                            case 2:
                                Register();
                                break;
                            default:
                   Console.WriteLine("Invalid choice. Please try again.");
                   Console.WriteLine("Press Enter to continue.");
                    Console.ReadLine();
                    Console.Clear();
                    break;
                        }
                    }
                    break;
                case 5:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.WriteLine("Press Enter to continue.");
                    Console.ReadLine();
                    Console.Clear();
                    break;
            }
        }
    }

    private void Login()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter password: ");
        string password = Console.ReadLine();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("Username or password cannot be empty.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            Console.Clear();
            return;
        }

        string storedPassword = ReadPassword(username);
        if (password == storedPassword)
        {
            loggedInUser = username;
            Console.WriteLine("Logged in successfully.");
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        Console.Clear();
    }

    private bool IsLoggedIn()
    {
        return !string.IsNullOrEmpty(loggedInUser);
    }

    private void Logout()
    {
        loggedInUser = null;
        Console.WriteLine("Logged out successfully.");
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
        Console.Clear();
    }
    public void Register()
    {
        Console.Write("Enter new username (at least 4 characters, only Latin letters and digits allowed): ");
        string newUsername = Console.ReadLine();

        if (newUsername.Length < 4) // RU: имя пользователя должно быть не менее 4 символов // ENG: nickname has to be at least 4 characters
        {
            Console.WriteLine("Username should be at least 4 characters long.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            Console.Clear();
            return;
        }
        if (!Regex.IsMatch(newUsername, "^[a-zA-Z0-9]+$"))
        {
            Console.WriteLine("Username should contain only Latin letters and digits."); // RU: имя пользователя должно быть написано на латинице (по желанию с цифрами) // ENG: username must be written in Latin (with numbers, if you wish)
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            Console.Clear();
            return;
        }

        Console.Write("Enter new password (at least 8 characters): ");
        string newPassword = Console.ReadLine();

        if (newPassword.Length < 8) // RU: пароль должен быть не менее 8 символов // ENG: password has to be at least 8 characters 
        {
            Console.WriteLine("Password should be at least 8 characters long.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            Console.Clear();
            return;
        }

        WritePassword(newUsername, newPassword);

        Console.WriteLine("Registration successful.");
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
        Console.Clear();
    }
    private string ReadPassword(string username)
    {
        if (File.Exists(PasswordsFilePath))
        {
            string[] lines = File.ReadAllLines(PasswordsFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == username)
                {
                    return parts[1];
                }
            }
        }
        return null;
    }

    private void WritePassword(string username, string password)
    {
        using (StreamWriter writer = File.AppendText(PasswordsFilePath))
        {
            writer.WriteLine($"{username}:{password}");
        }
    }

    private bool UserExists(string username)
    {
        if (File.Exists(PasswordsFilePath))
        {
            string[] lines = File.ReadAllLines(PasswordsFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == username)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SearchSongs()
    {
        Console.Write("Enter song name: ");
        string songName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(songName))
        {
            Console.WriteLine("Search query cannot be empty.");
            return;
        }
        try
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyC-LGZ7oafKvYUr0xz5bxx-7fOjthts79o", // RU: вставляем свой api ключ // ENG: put your api key here
                ApplicationName = "AITUtify"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = songName;
            searchListRequest.MaxResults = 1;
            searchListRequest.Type = "video"; 
            searchListRequest.VideoCategoryId = "10"; // RU: выводится ролики которые находятся в категории музыки // ENG: videos that are in the music category are displayed

            var searchListResponse = searchListRequest.Execute();
            var video = searchListResponse.Items.FirstOrDefault();
            if (video != null)
            {
                var videoId = video.Id.VideoId;
                var videoUrl = $"https://www.youtube.com/watch?v={videoId}";

                // RU: открывается URL в веб-браузере // ENG: opens URL in web browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = videoUrl,
                    UseShellExecute = true
                });

                // RU: доп. запрос для получения информации о видео // ENG: additional request for information about the video
                var videoRequest = youtubeService.Videos.List("snippet, contentDetails, statistics");
                videoRequest.Id = videoId;

                var videoResponse = videoRequest.Execute();
                var videoInfo = videoResponse.Items.FirstOrDefault();
                var viewCount = videoResponse.Items[0].Statistics.ViewCount;

                if (videoInfo != null)
                {
                    var snippet = videoInfo.Snippet;
                    var contentDetails = videoInfo.ContentDetails;

                    Console.WriteLine($"Title: {snippet.Title}"); // RU: название песни // ENG: song title
                    Console.WriteLine($"Published At: {snippet.PublishedAt}"); // RU: когда был выпущен ролик // ENG: when was the song video released
                    Console.WriteLine($"Channel Title: {snippet.ChannelTitle}"); // RU: название канала // ENG: channel name
                    var duration = XmlConvert.ToTimeSpan(contentDetails.Duration); // RU: конвертация времени, чтобы не писалось на подобии "P3M35S" // ENG: time conversion so that it is not written as "P3M35S"
                    string formattedDuration = $"{(int)duration.TotalMinutes} minutes {(int)duration.Seconds} seconds";
                    Console.WriteLine($"Duration: {formattedDuration}"); // RU: отображает длительность ролика // ENG: displays the duration of the video
                    Console.WriteLine($"Number of views on this video: {viewCount}"); // RU: отображает количество просмотров // ENG: displays the number of views

                    Console.WriteLine("Press Enter to clear logs and continue.");
                    Console.ReadLine();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Video information not found.");
                }
            }
            else
            {
                Console.WriteLine("No videos found for the specified song name.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
