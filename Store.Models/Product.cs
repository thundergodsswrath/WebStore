using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Store.Models;

public class Product
{
    [Key] public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    [DisplayName("Product Title")]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Developer { get; set; }
    [DisplayName("List Price")]
    [Range(1, 1000)]
    public double ListPrice { get; set; }
    [DisplayName("Price for 1-3 copies")]
    [Range(1, 1000)]
    public double Price { get; set; }
    [DisplayName("Price for 4+ copies")]
    [Range(1, 1000)]
    public double PriceFor4Copies { get; set; }

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    [ValidateNever]
    public Category Category { get; set; }
    [ValidateNever]
    public string ImageUrl { get; set; }
}