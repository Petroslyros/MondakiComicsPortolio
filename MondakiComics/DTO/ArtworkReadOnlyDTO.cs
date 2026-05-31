namespace MondakiComics.DTO
{
    public record ArtworkReadOnlyDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }
        public string? CategoryName { get; set; }
        public List<ArtworkImageReadOnlyDTO> Images { get; set; } = new();
        public DateTime InsertedAt { get; set; }
    }
}
