using Library.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
}
