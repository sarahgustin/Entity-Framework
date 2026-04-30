using BookstoreManagement.Data;
using BookstoreManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.Service;

public class AuthorService
{
    private readonly BookstoreDbContext _context;

    public AuthorService(BookstoreDbContext context)
    {
        _context = context;
    }
    
    public async Task<Author> CreateAuthorAsync(Author author)
    {
        if (string.IsNullOrWhiteSpace(author.Name))
        {
            throw new ArgumentException("Author name is required");
        }

        if (string.IsNullOrWhiteSpace(author.Email))
        {
            throw new ArgumentException ("Author email is required");
        }

        //cek email udah ada belum
        var existingAuthor = await _context.Author.FirstOrDefaultAsync(e => e.Email == author.Email);

        if (existingAuthor != null)
        {
            throw new InvalidOperationException($"Author with email {author.Email} already exists");
        }
        _context.Author.Add(author);
        await _context.SaveChangesAsync();
        return author;
    } 

    //get all author
    public async Task<List<Author>> GetAllAuthorAsync()
    {
        return await _context.Author
            .Include(a => a.Books)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    //get author by id
    public async Task<Author?> GetAuthorByIdAsync (int id)
    {
        return await _context.Author
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    //get author by most books count
    public async Task<Author?> GetAuthorWithMostBookAsync()
    {
        return await _context.Author
            .Include(a => a.Books)
            .OrderByDescending(a => a.Books.Count)
            .FirstOrDefaultAsync();
    }

    //update author    
    //tambah phone number sama alamat 
    public async Task<Author?> UpdateAuthorProfileAsync(int id, Author updatedAuthor)
    {
        var existingAuthor = await _context.Author.FindAsync(id);

        if (existingAuthor == null)
        {
            return null;
        }
        existingAuthor.PhoneNumber = updatedAuthor.PhoneNumber;
        existingAuthor.Address = updatedAuthor.Address;
        
        await _context.SaveChangesAsync();

        return existingAuthor;
    }

    //delete author by id
    public async Task<bool> DeleteAuthorAsync (int id)
    {
        var author = await _context.Author
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);


        if (author == null)
        {
            return false;
        }

        //cek author nya ada buku aua ngga
        if (author.Books.Any())
        {
            throw new InvalidOperationException ($"Gagal mengahapus author :  {author.Name}! Author ini masih memiliki buku yang terdaftar");
        }

        _context.Author.Remove(author);
        await _context.SaveChangesAsync();

        return true;
    }

}