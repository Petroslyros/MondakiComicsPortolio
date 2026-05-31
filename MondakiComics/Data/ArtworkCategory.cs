namespace MondakiComics.Data
{
    public class ArtworkCategory : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<Artwork> Artworks { get; set; } = new HashSet<Artwork>();
    }
}