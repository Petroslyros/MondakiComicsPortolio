using MondakiComics.Core.Enums;

namespace MondakiComics.Data
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public UserRole UserRole { get; set; }

        public virtual ICollection<Artwork> Artworks { get; set; } = new HashSet<Artwork>();
        public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new HashSet<ContactMessage>();
    }
}