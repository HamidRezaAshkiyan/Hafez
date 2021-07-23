using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezWPFUI.Helper;
using TcpListener = HafezLibrary.DataAccess.Processor.TcpListener;

namespace HafezWPFUI.Views.Windows
{
    public partial class ListenerWindowView
    {
        public System.Net.Sockets.TcpListener
            Server
        {
            get;
            set;
        } // = TcpListener.GetServer(IPAddress.Parse(TcpListener.GetLocalIpAddress()), GlobalConfig.PortNumber);

        public bool IsServerStarted { get; set; } = false; //NOT OPTIMAL

        public ListenerWindowView()
        {
            InitializeComponent();

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach ( IPAddress ip in host.AddressList )
            {
                ComboBoxAllIpAddresses.Items.Add(ip.ToString());
            }

            //Server.Start();
        }

        public async void StartReceiving()
        {
            try
            {
                if ( !IsServerStarted )
                {
                    Server = TcpListener.GetServer(IPAddress.Parse(TcpListener.GetLocalIpAddress()),
                                                   GlobalConfig.UserConfig.PortNumber);

                    StartServer();
                }

                while ( IsServerStarted )
                {
                    string   portInput      = await TcpListener.StartListening(Server);
                    string[] portInputSplit = portInput.Split('/');

                    UserModel networkUser = new UserModel
                    {
                        Id = -1, UserId = portInputSplit[0], Password = portInputSplit[1]
                        // Command = portInputSplit[2]
                    };

                    string command = portInputSplit[2];
                    for ( int i = 3; i < portInputSplit.Length; i++ )
                    {
                        command += $"/{portInputSplit[i]}";
                    }

                    networkUser         = networkUser.IsUserValid();
                    networkUser.Command = command;

                    if ( networkUser != null && networkUser.Id != -1 )
                    {
                        DataGridListener.Items.Add(networkUser);
                        await NetCommandProcessor.DoCommandAsync(networkUser);
                    }
                }
            }
            catch ( Exception e )
            {
                GlobalConfig.LogInformation(e);
            }
        }

        public void StopServer()
        {
            Server.Stop();
            IsServerStarted = false;
        }

        public void StartServer()
        {
            Server.Start();
            IsServerStarted = true;
        }

        private void MainGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListenerWindow_OnActivated(object sender, EventArgs e)
        {
            TextBlockInternet.Text = GetIpPort();
        }

        private void ButtonClearScreen_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DataGridListener.Items.Clear();
            }
            catch ( Exception exception )
            {
                GlobalConfig.LogInformation(exception);
            }
        }

        public static string GetIpPort(string ipAddress = "")
        {
            if ( string.IsNullOrWhiteSpace(ipAddress) )
            {
                ipAddress = TcpListener.GetLocalIpAddress();
            }

            string fullIpAddress = $"{ipAddress}:{GlobalConfig.UserConfig.PortNumber}";

            return fullIpAddress;
        }

        private void ComboBoxAllIpAddresses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StopServer();

            string? ipAddress = ComboBoxAllIpAddresses.SelectedItem.ToString();
            TextBlockInternet.Text = GetIpPort(ipAddress);
            Server = TcpListener.GetServer(IPAddress.Parse(ipAddress),
                                           GlobalConfig.UserConfig.PortNumber);

            StartServer();

            StartReceiving();
        }
    }
}