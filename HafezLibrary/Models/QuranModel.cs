using HafezLibrary.Controllers;

namespace HafezLibrary.Models
{
    public class QuranModel //: IHolyBook
    {
        public int ID { get; set; }
        public int SuraID { get; set; }
        public int AyahID { get; set; }
        public int PageNumber { get; set; }
        public string AyahText { get; set; }
        public string AyahPersianTranslation { get; set; }
        public string AyahEnglishTranslation { get; set; }

        public string SuraName => DbCache.SureLists[SuraID - 1].SuraName;
    }
}