using Microsoft.EntityFrameworkCore;
using proj1.Models;

namespace proj1.Data
{
    public class AddDbContext : DbContext
    {
        public AddDbContext(DbContextOptions<AddDbContext> options) : base(options) { }
       

        public DbSet<Todo> ToDoIt => Set<Todo>();

        public object Todo { get; internal set; }
    }
    
    
}
