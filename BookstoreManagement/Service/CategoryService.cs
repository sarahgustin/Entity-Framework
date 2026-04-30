using BookstoreManagement.Data;
using BookstoreManagement.Models;

using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.Service;

public class CategoriService
{
    private readonly BookstoreDbContext _context;

    public CategoriService(BookstoreDbContext context)
    {
        _context = context;
    }

    //create category
    // get semua category sama semua bukunya
    // gett category by id 
    // update category
    //delete category

    public async Task<Category> CreateCategoryAsync( Category category)
    {
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == category.Name);

        if (existingCategory != null)
        {
            throw new InvalidOperationException ($"Kategori {category.Name} sudah terdaftar");
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<List<Category>> GetaAllCategoriesWithBooksAsync()
    {
        return await _context.Categories
            .Include(c => c.Books)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> UpdateCategoryNameAsync(int id, Category updatedCategory)
    {
        var existingCategory = await _context.Categories.FindAsync(id);

        if(existingCategory == null)
        {
            return null;
        }

        existingCategory.Name = updatedCategory.Name;
        await _context.SaveChangesAsync();
        return existingCategory;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return false;
        }

        if (category.Books.Any())
        {
            throw new InvalidOperationException ($"Tidak dapat menghapus kategori {category.Name}! Masih memiliki buku terdaftar");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return true;
    }
}