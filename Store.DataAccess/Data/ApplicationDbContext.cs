using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.DataAccess.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category {Id = 1, Name = "Action", DisplayOrder = 1},
            new Category {Id = 2, Name = "Strategy", DisplayOrder = 2},
            new Category {Id = 3, Name = "Platformer", DisplayOrder = 3}
            );
        modelBuilder.Entity<Product>().HasData(
            new Product {Id = 1,
                Title = "Elden Ring",
                Description = "THE NEW FANTASY ACTION RPG. Rise, Tarnished, and be guided by grace to brandish the power of the Elden Ring and become an Elden Lord in the Lands Between.",
                Developer = "FromSoftware Inc.",
                ListPrice = 45,
                Price = 43,
                PriceFor4Copies = 38,
                CategoryId = 1,
                ImageUrl = ""
            },
            new Product {Id = 2,
                Title = "Cuphead",
                Description = "Cuphead is a classic run and gun action game heavily focused on boss battles. Inspired by cartoons of the 1930s, the visuals and audio are painstakingly created with the same techniques of the era, i.e. traditional hand drawn cel animation, watercolor backgrounds, and original jazz recordings.",
                Developer = "Studio MDHR Entertainment Inc.",
                ListPrice = 8,
                Price = 6,
                PriceFor4Copies = 5,
                CategoryId = 3,
                ImageUrl = ""
            },
            new Product {Id = 3,
                Title = "God Of War",
                Description = "His vengeance against the Gods of Olympus years behind him, Kratos now lives as a man in the realm of Norse Gods and monsters. It is in this harsh, unforgiving world that he must fight to survive… and teach his son to do the same.",
                Developer = "Santa Monica Studio",
                ListPrice = 30,
                Price = 28,
                PriceFor4Copies = 26,
                CategoryId = 1,
                ImageUrl = ""
            }
        );
    }
}