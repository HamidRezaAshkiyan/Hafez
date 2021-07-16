using Microsoft.Win32;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezWPFUI.Helper.Handlers;
using HafezWPFUI.Models;
using static HafezLibrary.Controllers.DbCache;
using static HafezLibrary.Controllers.PersonalDuaController;
using static HafezLibrary.DataAccess.Connector.ExcelConnector;

namespace HafezWPFUI.Views
{
    public partial class CustomPersonalDuaView
    {
        private string _personalDuaFilePath;

        // TODO bind path to txt select excel : move this to view model and bind there
        private const string DefaultPathString = "انتخاب فایل محتوای اکسل ادعیه";

        private string PersonalDuaFilePath
        {
            get => _personalDuaFilePath;
            set
            {
                _personalDuaFilePath = value;
                TxtPersonalDuaFilePath.Text = _personalDuaFilePath;
            }
        } //= DefaultPathString;

        public CustomPersonalDuaView()
        {
            InitializeComponent();

            PersonalDuaFilePath = DefaultPathString;
            UpdatePersonalDuaComboBoxes();
        }

        public void UpdatePersonalDuaComboBoxes()
        {
            ComboBoxPersonalDuaLists.Items.Clear();
            foreach ( var personalDuaList in GlobalConfig.PersonalDuaListsDisplay )
            {
                ComboBoxPersonalDuaLists.Items.Add(personalDuaList);
            }

            // ComboBoxPersonalDuaLists.ItemsSource = PersonalDuaLists;
        }

        private void ButtonBrowseDuaFile_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog {Filter = Properties.Resources.Excel_Document_Avalabale_Format};
                if ( ofd.ShowDialog() == true )
                {
                    PersonalDuaFilePath = ofd.FileName;
                }
            }
            catch ( Exception exception )
            {
                GlobalConfig.LogInformation(exception);
            }
        }

        private async void ButtonSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate
                if ( string.IsNullOrWhiteSpace(PersonalDuaName.Text) )
                {
                    MessageBoxHandler.ShowMessage("اخطار", "نام انتخاب شده معتبر نمیباشد.");
                    return;
                }

                if ( PersonalDuaFilePath == DefaultPathString )
                {
                    MessageBoxHandler.ShowMessage("اخطار", "شما هنوز فایلی را انتخاب نکرده اید.");
                    return;
                }

                var personalDuaList = new PersonalDuaListModel {DuaName = PersonalDuaName.Text};

                personalDuaList =
                    PersonalDuaListController.CreatePersonalDuaList(personalDuaList);

                var personalDuas =
                    await Task.Run(() => ImportPersonalDuaFromExcelToList(PersonalDuaFilePath));

                for ( var i = 0; i < personalDuas.Count; i++ )
                {
                    var personalDua = personalDuas[i];
                    personalDua.DuaId = personalDuaList.DuaId;
                    personalDua.FarazId = i + 1;
                }

                await ImportNotificationListDataAsync(personalDuas);

                MessageBoxHandler.ShowMessage("موفقیت", "ادعیه شما با موفقیت ذخیره شد.");

                UpdatePersonalDuas();
                GlobalConfig.UpdatePersonalDuaListsDisplay();

                var personalDuaListDisplay = GlobalConfig.PersonalDuaListsDisplay.Last();
                // Bootstrapper.ConfigureAutoMapper().Map<PersonalDuaListDisplayModel>(personalDuaList);

                GlobalConfig.Main.ComboBoxPersonalDuaNames.Items.Add(personalDuaListDisplay);
                ComboBoxPersonalDuaLists.Items.Add(personalDuaListDisplay);

                PersonalDuaFilePath = DefaultPathString;
                PersonalDuaName.Text = string.Empty;
            }
            catch ( Exception ex )
            {
                GlobalConfig.LogInformation(ex);
            }
        }

        private void ButtonDeletePersonalDua_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( ComboBoxPersonalDuaLists.SelectedItem is null )
                {
                    MessageBoxHandler.ShowMessage("اخطار", "شما هنوز ادعیه را انتخاب نکرده اید");
                    return;
                }

                var personalDuaListDisplay = ComboBoxPersonalDuaLists.SelectedItem as PersonalDuaListDisplayModel;
                var personalDuaList = Bootstrapper.ConfigureAutoMapper()
                    .Map<PersonalDuaListModel>(personalDuaListDisplay);

                PersonalDuaListController.RemovePersonalDua(personalDuaList);

                MessageBoxHandler.ShowMessage("موفقیت", "ادعیه شما با موفقیت حذف شد.");

                UpdatePersonalDuas();
                GlobalConfig.UpdatePersonalDuaListsDisplay();

                /*GlobalConfig.Main.ComboBoxPersonalDuaNames.Items.Remove(personalDuaListDisplay);
                ComboBoxPersonalDuaLists.Items.Remove(personalDuaListDisplay);*/

                UpdatePersonalDuaComboBoxes();
                GlobalConfig.Main.UpdatePersonalDuaComboBoxes();
            }
            catch ( Exception exception )
            {
                GlobalConfig.LogInformation(exception);
            }
        }
    }
}