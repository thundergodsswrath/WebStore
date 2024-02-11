using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}