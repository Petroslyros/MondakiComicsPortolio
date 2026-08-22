namespace MondakiComics.Data
{
    public class NewsPost : BaseEntity
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }
    }
}