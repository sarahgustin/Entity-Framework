using Microsoft.EntityFrameworkCore;
using BookstoreManagement.Models;

namespace BookstoreManagement.Data;

public class BookstoreDbContext : DbContext
{
    public DbSet<Book> Book {get; set;}
    public DbSet<Author> Author {get; set;}
    public DbSet<Category> Category {get; set;}

    //parameterless constructor
    public BookstoreDbContext()
    {
        
    }
    public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options)
    {
    }

    //set database conection

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=BookstoreDatabase.db");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Book>(entity =>
    {

        entity.HasIndex(b => b.Title).IsUnique();

     
        entity.HasOne(b => b.Author)
            .WithMany(a => a.Books) 
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict); 

        entity.HasMany(b => b.Categories)  
            .WithMany(c => c.Books);      
    });

    modelBuilder.Entity<Author>(entity =>
    {
        entity.HasIndex(e => e.Email).IsUnique();
        entity.HasIndex(n => n.PhoneNumber).IsUnique();
    });

    // --- KONFIGURASI CATEGORY ---
    modelBuilder.Entity<Category>(entity =>
    {
        entity.HasIndex(n => n.Name).IsUnique();
    });
    }
}