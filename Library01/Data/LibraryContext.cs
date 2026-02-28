using Library01.Models;
using Microsoft.EntityFrameworkCore;

namespace Library01.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=Library01DB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Book
            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(100);
                entity.Property(b => b.ISBN).HasMaxLength(20);
                entity.Property(b => b.QuantityInStock).HasDefaultValue(0);

                // Уникальность ISBN (если задан)
                entity.HasIndex(b => b.ISBN).IsUnique().HasFilter("[ISBN] IS NOT NULL");
            });

            // Author
            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.LastName).IsRequired().HasMaxLength(100);
                entity.Property(a => a.Country).HasMaxLength(100);

                // Уникальность: фамилия + имя + год рождения
                entity.HasIndex(a => new { a.LastName, a.FirstName, a.BirthYear }).IsUnique();
            });

            // Genre
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(100);
                entity.Property(g => g.Description).HasMaxLength(500);

                // Уникальность названия жанра
                entity.HasIndex(g => g.Name).IsUnique();
            });

            // BookAuthor (many-to-many)
            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasKey(ba => new { ba.BookId, ba.AuthorId });

                entity.HasOne(ba => ba.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(ba => ba.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ba => ba.Author)
                    .WithMany(a => a.BookAuthors)
                    .HasForeignKey(ba => ba.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // BookGenre (many-to-many)
            modelBuilder.Entity<BookGenre>(entity =>
            {
                entity.HasKey(bg => new { bg.BookId, bg.GenreId });

                entity.HasOne(bg => bg.Book)
                    .WithMany(b => b.BookGenres)
                    .HasForeignKey(bg => bg.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bg => bg.Genre)
                    .WithMany(g => g.BookGenres)
                    .HasForeignKey(bg => bg.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
