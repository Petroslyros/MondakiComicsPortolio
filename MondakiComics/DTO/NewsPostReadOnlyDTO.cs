namespace MondakiComics.DTO
{
    public class NewsPostReadOnlyDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }
        public DateTime InsertedAt { get; set; }
    }

    public class NewsPostInsertDTO
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsPublished { get; set; }
        public int SortOrder { get; set; }
    }

    public class NewsPostUpdateDTO
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool? IsPublished { get; set; }
        public int? SortOrder { get; set; }
    }
}