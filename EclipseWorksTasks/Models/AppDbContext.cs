using Microsoft.EntityFrameworkCore;
using EclipseWorksTasks.Models;

namespace EclipseWorksTasks.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            this.Database.ExecuteSqlRaw(File.ReadAllText("init-db.sql"));
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}