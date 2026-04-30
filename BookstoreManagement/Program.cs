using BookstoreManagement.Data;
using BookstoreManagement.Models;
using BookstoreManagement.Service;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement;

class Program
{
    static async Task Main(String [] args)
    {
        Console.WriteLine("=== Entity Framework Core Demo Application ===\n");

        using var context = new BookstoreDbContext();
        
        var bookService = new BookService(context);
        var AuthorService = new AuthorService(context);
        var Categories = new CategoriService(context);

         try
            {
                // Demo 1: Basic CRUD Operations
                await AuthorCrudOperationAsync(AuthorService);

                Console.WriteLine("\n=== Demo completed successfully! ===");
                Console.WriteLine("Check the CompanyDatabase.db file that was created in your project folder.");
                Console.WriteLine("You can open it with SQLite tools to see the actual database structure.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

    }
    static async Task AuthorCrudOperationAsync(AuthorService authorService)
    {
        Console.WriteLine("=== Author CRUD Operations Demo ===");
        Console.WriteLine("\n1. Creating a new employee...");
        var newAuthor = new Author
        {
            Name = "Jane",
            Email = "Jane@bookstore.com",
            PhoneNumber = "0955528191",
            Address = "Jakarta"
        };

        try
        {
            var createdAuthor = await authorService.CreateAuthorAsync(newAuthor);
            Console.WriteLine($"✓ Created Author: {createdAuthor.Name} (ID: {createdAuthor.Id})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error creating Author: {ex.Message}");
        }

        //read all authors
        Console.WriteLine("\n Menampilkan data seluruh author");
        var allAuthor = await authorService.GetAllAuthorAsync();
        Console.WriteLine($"\n Terdapat {allAuthor.Count} author");

        foreach (var author in allAuthor)
        {
            Console.WriteLine($" - {author.Name} memiliki {author.Books.Count} buku");
        }

        //get author by id
        if (allAuthor.Any())
        {
            var firsAuthor = allAuthor.First();
            var authorbyId = await authorService.GetAuthorByIdAsync(firsAuthor.Id);
            if( authorbyId != null)
            {
                Console.WriteLine("Menampilkan data author berdasarkan id");
                Console.WriteLine($"ID : {authorbyId.Id}");
                Console.WriteLine($"Nama  : {authorbyId.Name}");
            }

            //update author profile 
            Console.WriteLine("Update Profile Author");
            if (allAuthor.Any())
            {
                var authorUpdate = allAuthor.Last();
                
                authorUpdate.Address = "Bandung";
                authorUpdate.PhoneNumber = "087652522";

                var updatedAuthor = await authorService.UpdateAuthorProfileAsync(authorUpdate.Id, authorUpdate);

                if (updatedAuthor != null)
                {
                    Console.WriteLine($"Update profile author : {updatedAuthor.Name}");
                    Console.WriteLine($"Alamat : {updatedAuthor.Address}");
                    Console.WriteLine($"Nomor HP : {updatedAuthor.PhoneNumber}");
                }
            }

        }
            Console.WriteLine("\n--- CRUD Operations Demo Complete ---\n");

    }

    static async Task CategotyCrudOperationAsync(CategoriService categoriService)
    {
         Console.WriteLine("=== Category CRUD Operations Demo ===");

         //create categoty
         Console.WriteLine("Menambahkan kateori baru");
         var newCategory = new Category
         {
             Name = "Horror"
         };

        try
        {
            var createdCategory = await categoriService.CreateCategoryAsync(newCategory);
            Console.WriteLine($"✓ Created Category: {createdCategory.Name} (ID: {createdCategory.Id})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error creating Author: {ex.Message}");
        }

        //get all category 
        Console.WriteLine("Menampilkan seluruh data kategori");
        var allCategory = await categoriService.GetaAllCategoriesWithBooksAsync();
        Console.WriteLine($"Terdapat {allCategory.Count}");

        foreach (var category in allCategory)
        {
            
        }
    }
}

