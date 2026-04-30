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




}