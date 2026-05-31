namespace MondakiComics.DTO
{
    public record ArtworkImageReadOnlyDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
    }
}
