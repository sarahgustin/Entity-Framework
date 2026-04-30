using Microsoft.EntityFrameworkCore;
using BookstoreManagement.Models;

namespace BookstoreManagement.Data;

public class BookstoreDbContext : DbContext
{
    public DbSet<Book> Books {get; set;}
    public DbSet<Author> Authors {get; set;}
    public DbSet<Category> Categories {get; set;}

    //parameterless constructor
    public BookstoreDbContext()
    {
        
    }

    //set database conection
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Configure kalo belum di configure
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=BookstoreDatabase.db");
        }
        //base.OnConfiguring(optionsBuilder);
    }

    //bikin model pake override OnModelCreating

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //book
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(b => b.Title).IsUnique();
            entity.HasOne(a => a.Author)
                .WithMany(b => b.Books)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);            
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(c => c.Categories).IsUnique();
            entity.HasMany(c => Categories)
                .WithMany(b => b.Books);
        });

        //author
        modelBuilder.Entity<Author> (entity =>
        {
            entity.HasIndex (e=> e.Email).IsUnique();
            entity.HasIndex(n => n.PhoneNumber).IsUnique();
        });

        //category
        modelBuilder.Entity<Category> (entity =>
        {
            entity.HasIndex(n => n.Name).IsUnique();
        });

    }
}