namespace MondakiComics.Data
{
    public class Artwork : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool IsPublished { get; set; } = false;
        public int SortOrder { get; set; } = 0;

        public virtual User User { get; set; } = null!;
        public virtual ArtworkCategory? Category { get; set; }
        public virtual ICollection<ArtworkImage> Images { get; set; } = new HashSet<ArtworkImage>();
    }
}