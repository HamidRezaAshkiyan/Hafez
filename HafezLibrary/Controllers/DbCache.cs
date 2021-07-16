using System.Collections.Generic;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class DbCache
    {
        public static IEnumerable<QuranModel> Quran { get; } = QuranController.GetAllQuran();
        public static List<SureListModel> SureLists { get; } = SureListController.GetAllSureList();
        public static List<MafatihModel> Mafatih { get; } = MafatihController.GetAllMafatih();
        public static IEnumerable<MafatihListModel> MafatihLists { get; } = MafatihListController.GetAllDuas();

        public static List<PersonalDuaModel> PersonalDuas { get; private set; } = PersonalDuaController.GetAllPersonalDuas();
        public static IEnumerable<PersonalDuaListModel> PersonalDuaLists { get; private set; } = PersonalDuaListController.GetAllPersonalDuaList();

        public static void UpdatePersonalDuaLists()
        {
            PersonalDuaLists = PersonalDuaListController.GetAllPersonalDuaList();
        }

        public static void UpdatePersonalDuas()
        {
            PersonalDuas = PersonalDuaController.GetAllPersonalDuas();
        }
    }
}