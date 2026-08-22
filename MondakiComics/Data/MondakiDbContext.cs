using Microsoft.EntityFrameworkCore;

namespace MondakiComics.Data
{
    public class MondakiDbContext : DbContext
    {
        public MondakiDbContext(DbContextOptions<MondakiDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ArtworkCategory> ArtworkCategories { get; set; } = null!;
        public DbSet<Artwork> Artworks { get; set; } = null!;
        public DbSet<ArtworkImage> ArtworkImages { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public DbSet<NewsPost> NewsPosts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.UserRole).HasConversion<string>().HasMaxLength(20);
                entity.Property(e => e.InsertedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");
                entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
                entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();
            });

            modelBuilder.Entity<ArtworkCategory>(entity =>
            {
                entity.ToTable("ArtworkCategories");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.InsertedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");
                entity.HasIndex(e => e.Slug, "IX_ArtworkCategories_Slug").IsUnique();
            });

            modelBuilder.Entity<Artwork>(entity =>
            {
                entity.ToTable("Artworks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(2000);
                entity.Property(e => e.CoverImageUrl).HasMaxLength(1000);
                entity.Property(e => e.InsertedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");
                entity.Property(e => e.ModifiedAt).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("NOW()");

                entity.HasOne(a => a.User)
                      .WithMany(u => u.Artworks)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Category)
                      .WithMany(c => c.Artworks)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ArtworkImage>(entity =>
            {
                entity.ToTable("ArtworkImages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.AltText).HasMaxLength(255);
                entity.Property(e => e.InsertedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");

                entity.HasOne(i => i.Artwork)
                      .WithMany(a => a.Images)
                      .HasForeignKey(i => i.ArtworkId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.ToTable("ContactMessages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SenderName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.SenderEmail).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Message).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.ReceivedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");

                entity.HasOne(m => m.User)
                      .WithMany(u => u.ContactMessages)
                      .HasForeignKey(m => m.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NewsPost>(entity =>
            {
                modelBuilder.Entity<NewsPost>(entity =>
                {
                    entity.ToTable("NewsPosts");
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Title).HasMaxLength(255).IsRequired();
                    entity.Property(e => e.Content).HasMaxLength(4000).IsRequired();
                    entity.Property(e => e.ImageUrl).HasMaxLength(1000);
                    entity.Property(e => e.InsertedAt).ValueGeneratedOnAdd().HasDefaultValueSql("NOW()");
                    entity.Property(e => e.ModifiedAt).ValueGeneratedOnAddOrUpdate().HasDefaultValueSql("NOW()");
                });
            });
        }
    }
}