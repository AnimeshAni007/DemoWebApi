using Microsoft.EntityFrameworkCore;

namespace Demo // Make sure this matches your project's namespace
{
    public class AppDbContext : DbContext
    {
        // This constructor passes your connection settings down into the Entity Framework core engine
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // This tells .NET to create a table named "Products" mapped to your internal data object
        public DbSet<ProductItem> Products { get; set; }

        public DbSet<UserItem> Users { get; set; }
    }

    // This defines the structure of your database table rows
    public class ProductItem
    {
        public int Id { get; set; } // EF Core automatically makes 'Id' an auto-incrementing Primary Key
        public string Title { get; set; } = string.Empty;
    }

    public class UserItem
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Note: In a production app, never store plain text passwords!
        public string Role { get; set; } = "User"; // Can be "Admin" or "User"
    }
}
