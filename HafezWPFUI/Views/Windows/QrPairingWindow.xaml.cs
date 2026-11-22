using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using HafezWPFUI.Services;
using QRCoder;

namespace HafezWPFUI.Views.Windows
{
    public partial class QrPairingWindow
    {
        public QrPairingWindow(HafezRemoteServer remoteServer)
        {
            InitializeComponent();
            RenderQr(remoteServer);
        }

        private void RenderQr(HafezRemoteServer remoteServer)
        {
            string payload = remoteServer.GetPairingQrJson();
            using var generator = new QRCodeGenerator();
            using QRCodeData qrData = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var pngQr = new PngByteQRCode(qrData);
            byte[] pngBytes = pngQr.GetGraphic(10);

            var bitmap = new BitmapImage();
            using (var ms = new MemoryStream(pngBytes))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            QrImage.Source = bitmap;
            EndpointText.Text = $"http://{remoteServer.LocalIp}:{remoteServer.Port}";
            ClientsText.Text = $"دستگاه‌های متصل: {remoteServer.CurrentState.ConnectedClients}";
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
