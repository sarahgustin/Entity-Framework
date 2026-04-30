using System.ComponentModel;
using System.Data.Common;
using System.Windows.Markup;
using BookstoreManagement.Data;
using BookstoreManagement.Models;

using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.Service;

public class BookService
{
    private readonly BookstoreDbContext _context;

    public BookService(BookstoreDbContext context)
    {
        _context = context;
    }

    //create buku
    //pilih kategori
    //get buku sama detail categori
    //get buku by author
    //get buku by categori
    //update book -> price
    //delete book 

    public async Task<Book> CreateBookAsync(Book book, List<int> CategoryId)
    {
        var existingAuthor = await _context.Author.AnyAsync(a => a.Id == book.AuthorId);
        if(!existingAuthor)
        {
            throw new InvalidOperationException ($"Author dengan id {book.AuthorId} tidak ditemukan");
        }

        var selectedCategory = await _context.Category
            .Where(c => CategoryId.Contains(c.Id))
            .ToListAsync();
        if (selectedCategory.Count == 0)
        {
            throw new InvalidOperationException("Kategori yang dipilih tidak valid");
        }
        book.Categories = selectedCategory;

        _context.Book.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }
    public async Task<List<Book>> GetAllBookWithAuthorandCategoryAsync()
    {
        return await _context.Book
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .ToListAsync();
    }
    
    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _context.Book
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> UpdateBookPriceAsync (int id, Book updatedBook)
    {
        var existingBook = await _context.Book.FindAsync(id);

        if (existingBook == null)
        {
            return null;
        }

        existingBook.Title = updatedBook.Title;
        existingBook.Price = updatedBook.Price;

        await _context.SaveChangesAsync();
        return updatedBook;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Book.FindAsync(id);

        if (book == null)
        {
            return false;
        }

        _context.Book.Remove(book);
        await _context.SaveChangesAsync();

        return true;
    }

    
    

}