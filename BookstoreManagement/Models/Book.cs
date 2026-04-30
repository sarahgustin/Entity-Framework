using System.ComponentModel.DataAnnotations;

namespace BookstoreManagement.Models;

public class Book
{
    //id
    public int Id {get; set;}

    //judul
    [Required]
    [MaxLength(300)]
    public string Title {get; set;} = string.Empty;
    
    //deskripsi
    [MaxLength(500)]
    public string? Description {get; set;}
    //harga
    public decimal Price {get; set;}

    //waktu rilis
    public DateTime Release {get; set;}
    
    //author
    [Required]
    public int?AuthorId {get; set;}
    public virtual Author Author {get; set;} = null!;
   
    //kategori
    public virtual ICollection<Category> Categories {get; set;} = new List<Category>();
    
}