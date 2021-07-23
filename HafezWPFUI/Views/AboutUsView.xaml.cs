using System.Diagnostics;
using System.Reflection;
using HafezLibrary.DataAccess.Connector;

namespace HafezWPFUI.Views
{
    public partial class AboutUsView
    {
        public AboutUsView()
        {
            InitializeComponent();

            //var customerName = "تست"; //TinyxConnector.CustomerName;

            TxtVersion.Text = $"شماره نسخه: {Assembly.GetExecutingAssembly().GetName().Version}";
            //TxtLicense.Text =
            //    $"این محصول توسط \"{customerName}\" خریداری شده است. تنها خریدار مجاز به استفاده از آن میباشد و پشتیبانی از این محصول تنها برای خریدار ارائه میگردد.\n";
        }

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Process.Start("https://idpay.ir/hafez-software");
        }
    }
}