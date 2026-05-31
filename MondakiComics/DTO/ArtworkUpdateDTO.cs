namespace MondakiComics.DTO
{
    public record ArtworkUpdateDTO
    {
        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool? IsPublished { get; set; }
        public int? SortOrder { get; set; }
    }
}
