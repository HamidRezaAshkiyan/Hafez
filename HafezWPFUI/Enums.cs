namespace HafezWPFUI
{
    public static class Enums
    {
        public enum PanelTypes
        {
            Notification       = 1,
            Quran              = 2,
            Mafatih            = 3,
            QuranTranslation   = 4,
            MafatihTranslation = 5
        }

        public enum AnimationMode
        {
            Static,
            Continuous,
            Separately
        }

        public enum UserTypes
        {
            SuperAdmin,
            Admin,
            User
        }

        public enum NotificationType
        {
            Network = 'N',
            Local   = 'L'
        }
    }
}