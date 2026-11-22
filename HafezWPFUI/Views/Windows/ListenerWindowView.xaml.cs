using System;
using System.Net;
using System.Threading;
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
        private CancellationTokenSource _cts;

        public System.Net.Sockets.TcpListener Server { get; set; }

        public bool IsServerStarted { get; set; }

        public ListenerWindowView()
        {
            InitializeComponent();

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                ComboBoxAllIpAddresses.Items.Add(ip.ToString());
            }
        }

        public async void StartReceiving()
        {
            try
            {
                if (!IsServerStarted)
                {
                    Server = TcpListener.GetServer(IPAddress.Parse(TcpListener.GetLocalIpAddress()),
                                                   GlobalConfig.UserConfig.PortNumber);

                    StartServer();
                }

                CancellationToken token = _cts?.Token ?? CancellationToken.None;

                while (IsServerStarted && !token.IsCancellationRequested)
                {
                    string portInput = await TcpListener.StartListening(Server, token);
                    if (!TcpListener.TryParseCommandPacket(portInput, out string userId, out string password, out string command))
                    {
                        continue;
                    }

                    UserModel networkUser = new()
                    {
                        Id = -1,
                        UserId = userId,
                        Password = password
                    };

                    networkUser = networkUser.IsUserValid();
                    if (networkUser == null || networkUser.Id == -1)
                    {
                        continue;
                    }

                    networkUser.Command = command;
                    DataGridListener.Items.Add(networkUser);
                    await NetCommandProcessor.DoCommandAsync(networkUser);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when StopServer cancels the listener loop.
            }
            catch (ObjectDisposedException)
            {
                // Expected when the underlying socket is stopped.
            }
            catch (Exception e)
            {
                GlobalConfig.LogInformation(e);
            }
        }

        public void StopServer()
        {
            IsServerStarted = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            Server?.Stop();
        }

        public void StartServer()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
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
            catch (Exception exception)
            {
                GlobalConfig.LogInformation(exception);
            }
        }

        public static string GetIpPort(string ipAddress = "")
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                ipAddress = TcpListener.GetLocalIpAddress();
            }

            return $"{ipAddress}:{GlobalConfig.UserConfig.PortNumber}";
        }

        private void ComboBoxAllIpAddresses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxAllIpAddresses.SelectedItem == null)
            {
                return;
            }

            StopServer();

            string ipAddress = ComboBoxAllIpAddresses.SelectedItem.ToString();
            if (!IPAddress.TryParse(ipAddress, out IPAddress parsedIp))
            {
                return;
            }

            TextBlockInternet.Text = GetIpPort(ipAddress);
            Server = TcpListener.GetServer(parsedIp, GlobalConfig.UserConfig.PortNumber);

            StartServer();
            StartReceiving();
        }
    }
}
