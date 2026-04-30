using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BookstoreManagement.Models;

public class Category
{
    public int Id {get; set;}

    [Required]
    [MaxLength(100)]
    public string Name {get;set;} = string.Empty;

    public virtual ICollection<Book> Books {get;set;} = new List<Book>();

}