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

        await context.Database.MigrateAsync();

        await SeedDatabaseAsync(context);
        
        var bookService = new BookService(context);
        var authorService = new AuthorService(context);
        var categoryService = new CategoriService(context);

         try
            {
                // Jalankan berurutan: Author -> Category -> Book
                await AuthorCrudOperationAsync(authorService);
                await CategotyCrudOperationAsync(categoryService);
                await BookCategoryCrudOperaionAsync(bookService, authorService, categoryService);

                Console.WriteLine("\n=== Demo completed successfully! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError occurred: {ex.Message}");
            }

    }
   static async Task AuthorCrudOperationAsync(AuthorService authorService)
    {
        Console.WriteLine("\n=== Author CRUD Operations Demo ===");
        
        //CREATE AUTHOR
        Console.WriteLine("\n1. Menambahkan Author baru");
        var newAuthor = new Author
        {
            Name = "J.K Rowling",
            Email = "jk.rowling@bookstore.com",
            PhoneNumber = "0955528191",
            Address = "United Kingdom"
        };

        try
        {
            var createdAuthor = await authorService.CreateAuthorAsync(newAuthor);
            Console.WriteLine($"   ✓ Berhasil: {createdAuthor.Name} (ID: {createdAuthor.Id})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ✗ Gagal membuat Author: {ex.Message}");
        }

        //READ ALL AUTHOR
        Console.WriteLine("\n2. Menampilkan seluruh data Author");
        var allAuthor = await authorService.GetAllAuthorAsync();
        Console.WriteLine($"   Total: {allAuthor.Count} author");

        foreach (var author in allAuthor)
        {
            Console.WriteLine($"   - ID: {author.Id} | {author.Name} ({author.Books?.Count ?? 0} buku)");
        }

        if (allAuthor.Any())
        {
            //READ BY ID
            var firstAuthor = allAuthor.First();
            var authorById = await authorService.GetAuthorByIdAsync(firstAuthor.Id);
            if (authorById != null)
            {
                Console.WriteLine($"\n3. Detail Author ID {firstAuthor.Id}:");
                Console.WriteLine($"   Nama   : {authorById.Name}");
                Console.WriteLine($"   Email  : {authorById.Email}");
            }

            //READ AUTHOR WITH MOST BOOK
            Console.WriteLine("\n4. Mencari Author dengan koleksi buku terbanyak");
            var topAuthor = await authorService.GetAuthorWithMostBookAsync();
            if (topAuthor != null)
            {
                Console.WriteLine($"   - Author: {topAuthor.Name} ({topAuthor.Books?.Count ?? 0} buku)");
            }
            else
            {
                Console.WriteLine("   - Data buku belum tersedia.");
            }

            //UPDATE AUTHOR PROFILE
            Console.WriteLine("\n4. Update Profile Author (mengambil data terakhir)");
            var authorToUpdate = allAuthor.Last();
            authorToUpdate.Address = "London";
            authorToUpdate.PhoneNumber = "08123456789";

            var updatedAuthor = await authorService.UpdateAuthorProfileAsync(authorToUpdate.Id, authorToUpdate);
            if (updatedAuthor != null)
            {
                Console.WriteLine($"   ✓ Berhasil Update: {updatedAuthor.Name}");
                Console.WriteLine($"     Alamat Baru: {updatedAuthor.Address}");
            }
            
            //DELETE AUTHOR
            Console.WriteLine($"\n5. Mencoba menghapus Author ID: {authorToUpdate.Id}...");
            try
            {
                var isDeleted = await authorService.DeleteAuthorAsync(authorToUpdate.Id);
                if (isDeleted)
                    Console.WriteLine("   ✓ Berhasil menghapus Author.");
                else
                    Console.WriteLine("   ✗ Author tidak ditemukan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Gagal Hapus: {ex.Message}");
            }
        }
        Console.WriteLine("\n--- Author Operations Complete ---");
    }

    static async Task CategotyCrudOperationAsync(CategoriService categoryService)
    {
        Console.WriteLine("\n=== Category CRUD Operations Demo ===");

        //CREATE CATEGORY
        Console.WriteLine("\n1. Menambahkan kategori baru...");
        string[] newCategories = { "Fantasy", "Fiction", "Adventure" };

       foreach(var category in newCategories)
        {
            try
            {
                var createdCategory = await categoryService.CreateCategoryAsync(new Category { Name = category });
                Console.WriteLine($"   ✓ Berhasil: {createdCategory.Name} (ID: {createdCategory.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Gagal: {ex.Message}");
            }
        }
        //READ ALL CATEGORY
        var allCategory = await categoryService.GetaAllCategoriesWithBooksAsync();
        Console.WriteLine($"\n2. Total {allCategory.Count} kategori terdaftar:");
        foreach (var category in allCategory)
        {
            Console.WriteLine($"   - [{category.Id}] {category.Name}");
        }

        if (allCategory.Any())
        {
            //READ BY ID
            var category = allCategory.First();
            var categoryId = await categoryService.GetCategoryByIdAsync(category.Id);

            if (categoryId != null)
            {
                Console.WriteLine("\n3. Menampilkan kategori berdasarkan id");
                Console.WriteLine($"ID : {categoryId.Id}");
                Console.WriteLine($"Kategori : {categoryId.Name}");
            }

            //UPDATE CATEGORY
            var categoryToUpdate = allCategory.Last();
            Console.WriteLine($"\n4. Update kategori '{categoryToUpdate.Name}' menjadi 'Fantasy'...");
            
            categoryToUpdate.Name = "Horror";
            var updated = await categoryService.UpdateCategoryNameAsync(categoryToUpdate.Id, categoryToUpdate);
            
            if (updated != null)
                Console.WriteLine($"   ✓ Berhasil Update ID {updated.Id}");


            //DELETE
            Console.WriteLine($"\n5. Menghapus kategori ID {categoryToUpdate.Id}...");
            try
            {
                var isDeleted = await categoryService.DeleteCategoryAsync(categoryToUpdate.Id);
                if (isDeleted)
                Console.WriteLine("   ✓ Berhasil menghapus kategori.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Gagal Hapus: {ex.Message}");
            }
        }
        Console.WriteLine("\n--- Category Operations Complete ---");
    }
        
    

    static async Task BookCategoryCrudOperaionAsync(BookService bookService, AuthorService authorService, CategoriService categoryService)
    {
        Console.WriteLine("\n=== Book CRUD Operations Demo ===");

        var getAllAuthor = await authorService.GetAllAuthorAsync();
        var getAllCategory = await categoryService.GetaAllCategoriesWithBooksAsync();
        
        if(!getAllAuthor.Any() ||!getAllCategory.Any())
        {
            Console.WriteLine("   ✗ Gagal: Kamu harus punya minimal 1 Author dan 1 Kategori dulu!");
            return;
        }

        var targetAuthor = getAllAuthor.First();
        var targetCategoryIds = getAllCategory
            .Where(c => c.Name == "Fantasy" || c.Name == "Fiction" || c.Name == "Horror")
            .Select(c => c.Id)
            .ToList();

        //CREATE AUTHOR
        Console.WriteLine("\n1. Menambahkan Author baru...");
        var newBook = new Book
        {
            Title = "Harry Potter and the Philosopher's Stone",
            Description = "A young boy discovers he is a wizard on his eleventh birthday.",
            Price = 175000,
            AuthorId = targetAuthor.Id
        };

        try
        {
            Console.WriteLine($"\n1. Menambahkan buku '{newBook.Title}' untuk Author: {targetAuthor.Name}...");
            // Panggil method Create yang sudah kamu buat
            var createdBook = await bookService.CreateBookAsync(newBook, targetCategoryIds);
            
           Console.WriteLine($"   ✓ Berhasil ditambahkan!");
           Console.WriteLine($"     Kategori: {string.Join(", ", createdBook.Categories.Select(c => c.Name))}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ✗ Error: {ex.Message}");
        }

        //READ ALL BOOK LIST
        Console.WriteLine("\n2. Menampilkan seluruh data Buku");
        var allBooks = await bookService.GetAllBookWithAuthorandCategoryAsync();
        Console.WriteLine($"   Total : {allBooks.Count} buku");

        foreach(var book in allBooks)
        { 
            var authorName = book.Author?.Name ?? "Tanpa Author";
            var categoryNames = book.Categories != null && book.Categories.Any()
                ? string.Join(", ", book.Categories.Select(c => c.Name))
                : "Tanpa Kategori";
            Console.WriteLine($"   - ID: {book.Id} | {book.Title} | Author: {authorName} | Kategori: [{categoryNames}] | Rp {book.Price}");
        }

        if (allBooks.Any())
        {
            //READ BOOK DATA BY ID
            var firstBook = allBooks.First();
            var bookById = await bookService.GetBookByIdAsync(firstBook.Id);
            
            if(bookById != null)
            {
                Console.WriteLine($"\n3. Detail Buku ID {firstBook.Id}:");
                Console.WriteLine($"   Judul   : {firstBook.Title}");
                Console.WriteLine($"   Author  : {firstBook.Author}");
                Console.WriteLine($"   Deskripsi : {firstBook.Description}");
                Console.WriteLine($"   Harga : {firstBook.Price}");
            }

            //UPDATE BOOK DETAIL
            Console.WriteLine("\n4. Update detail buku");
            var bookUpdate = allBooks.Last();
            bookUpdate.Title = "Harry Potter and the Sorcerer's Stone";
            bookUpdate.Price = 200000;

            var updatedBook = await bookService.UpdateBookPriceAsync(bookUpdate.Id, bookUpdate);
            if (updatedBook != null)
            {
                Console.WriteLine($"   ✓ Berhasil Update Buku : {allBooks.Last().Title}");
                Console.WriteLine($"     Judul Baru: {updatedBook.Title}");
                Console.WriteLine($"     Harga Telpon Baru: {updatedBook.Price}");
            }

            //DELETE BOOK
            Console.WriteLine($"\n5. Mencoba menghapus Book ID: {bookUpdate.Id}");
             try
            {
                var isDeleted = await bookService.DeleteBookAsync(bookUpdate.Id);
                if (isDeleted)
                    Console.WriteLine("   ✓ Berhasil menghapus buku.");
                else
                    Console.WriteLine("   ✗ Author tidak ditemukan.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ✗ Gagal Hapus: {ex.Message}");
            }

        }


    }

    static async Task SeedDatabaseAsync(BookstoreDbContext context)
    {
        if (await context.Author.AnyAsync())
        {
            Console.WriteLine("Database sudah berisi data, melewati proses seeding.\n");
            return;
        }

        Console.WriteLine("Sedang mengisi data awal (Seeding)...");

        // 1. Seed Authors
        var authors = new[]
        {
            new Author { Name = "Paulo Coelho", Email = "paulo@bookstore.com", PhoneNumber = "0111", Address = "UK" },
            new Author { Name = "Haemin Sunim", Email = "haemin@bookstore.com", PhoneNumber = "0222", Address = "Soul" },
            new Author { Name = "Tere Liye", Email = "tereliye@bookstore.com", PhoneNumber = "0333", Address = "Jakarta" }
        };
        context.Author.AddRange(authors);
        await context.SaveChangesAsync();

        var categories = new[]
        {
            new Category { Name = "Fantasy" },
            new Category { Name = "Fiction" },
            new Category { Name = "Adventure" },
            new Category { Name = "Drama" },
            new Category { Name = "Self Improvement"}
        };
        context.Category.AddRange(categories);
        await context.SaveChangesAsync();

       // Ambil data yang baru saja di-seed untuk relasi
    var allAuthors = await context.Author.ToListAsync();
    var allCategories = await context.Category.ToListAsync();

    // 3. Seed Books
    var books = new List<Book>
    {
        new Book 
        { 
            Title = "The Things You Can See Only When You Slow Down", 
            Description = "How to be calm in a busy world.", 
            Price = 150000, 
            AuthorId = allAuthors.First(a => a.Name == "Haemin Sunim").Id,
            Categories = allCategories.Where(c => c.Name == "Self Improvement").ToList()
        },
        new Book 
        { 
            Title = "The Alchemist", 
            Description = "A journey to find worldly treasure.", 
            Price = 120000, 
            AuthorId = allAuthors.First(a => a.Name == "Paulo Coelho").Id,
            Categories = allCategories.Where(c => c.Name == "Fiction" || c.Name == "Drama").ToList()
        }
    };

    context.Book.AddRange(books);
    await context.SaveChangesAsync();

        Console.WriteLine("Database seeding selesai!\n");
    }

      static async Task ApplyMigrationsAsync(BookstoreDbContext context)
        {
            Console.WriteLine("Applying database migrations...");
            
            // Apply any pending migrations
            await context.Database.MigrateAsync();
            
            Console.WriteLine("Database migrations applied successfully!\n");
        }

}

