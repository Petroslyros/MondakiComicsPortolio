namespace MondakiComics.Data
{
    public class ContactMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; } = false;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}