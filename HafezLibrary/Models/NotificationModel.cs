namespace HafezLibrary.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int SortId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public char NotificationType { get; set; }
        public string CreatedAt { get; set; } //= DateTime.UtcNow.ToString();
        public int CreatedBy { get; set; }

        //custom
        public string CreatorName { get; set; }
    }
}