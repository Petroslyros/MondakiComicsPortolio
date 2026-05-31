namespace MondakiComics.DTO
{
    public record ContactMessageReadOnlyDTO
    {
        public int Id { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
