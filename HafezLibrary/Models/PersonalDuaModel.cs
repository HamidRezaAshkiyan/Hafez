namespace HafezLibrary.Models
{
    public class PersonalDuaModel
    {
        public int    Id          { get; set; }
        public int    DuaId       { get; set; }
        public int    FarazId     { get; set; }
        public string ArabicText  { get; set; }
        public string PersianText { get; set; }
    }
}