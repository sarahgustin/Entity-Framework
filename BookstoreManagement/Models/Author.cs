using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.Models;

public class Author
{
    public int Id {get; set;} 

    [Required]
    [MaxLength(200)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;
    
    [MaxLength(500)]
    public string Address {get;set;} = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber {get;set;}

    public virtual ICollection<Book> Books {get; set;} = new List<Book>();

}