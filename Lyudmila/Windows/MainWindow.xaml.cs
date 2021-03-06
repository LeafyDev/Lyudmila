﻿// -----------------------------------------------------------
// Copyrights (c) 2016 Seditio 🍂 INC. All rights reserved.
// -----------------------------------------------------------

using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Lyudmila.Helpers;
using Lyudmila.Properties;

using Newtonsoft.Json.Linq;

namespace Lyudmila.Windows
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GetServerIP()
        {
            Notify("Searching for server...");
            NetTools.Init();
            do
            {
                await Task.Delay(1000);
            }
            while(string.IsNullOrEmpty(Settings.Default.ServerIP));

            Notify($"Found server at {Settings.Default.ServerIP}");

            LoadGames();
        }

        private void LoadGames()
        {
            try
            {
                var update = new WebClient().DownloadString(new Uri($"http://{Settings.Default.ServerIP}:8080/Static/games.json"));
                
                dynamic data = JObject.Parse(update);

                foreach(var game in data.EnabledGames)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ListBox_Games.Items.Add(game.Value.longname);
                    });
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.GetType()}: {ex.Message}");
            }
        }

        private async void Notify(string message)
        {
            Notifier.Display(message);
            Dispatcher.Invoke(() => { NotificationFlyout.IsOpen = true; });

            await Task.Delay(2000);

            Dispatcher.Invoke(() =>
            {
                if(NotificationFlyout.IsOpen)
                {
                    NotificationFlyout.IsOpen = false;
                }
            });
        }

        private void ShowSettings(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowFriends(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ShowMusic(object sender, RoutedEventArgs e)
        {
            MusicFlyout.IsOpen = !MusicFlyout.IsOpen;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Settings.Default.Username))
            {
                new Thread(WaitForNickname).Start();
            }
            else
            {
                new Thread(GetServerIP).Start();
            }
        }

        private void WaitForNickname()
        {
            Dispatcher.Invoke(() =>
            {
                NicknameFlyout.IsOpen = true;
                NicknameFlyout.ClosingFinished += NickCheck;
            });
        }

        private void NickCheck(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(Settings.Default.Username))
            {
                NicknameFlyout.IsOpen = true;
            }
            else
            {
                new Thread(GetServerIP).Start();
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e) => Environment.Exit(0);
    }
}