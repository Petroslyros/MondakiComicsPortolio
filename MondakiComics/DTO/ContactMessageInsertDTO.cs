namespace MondakiComics.DTO
{
    public record ContactMessageInsertDTO
    {
        public string SenderName { get; set; } = null!;
        public string SenderEmail { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
