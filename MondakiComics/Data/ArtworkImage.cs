namespace MondakiComics.Data
{
    public class ArtworkImage
    {
        public int Id { get; set; }
        public int ArtworkId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? AltText { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime InsertedAt { get; set; } = DateTime.UtcNow;

        public virtual Artwork Artwork { get; set; } = null!;
    }
}