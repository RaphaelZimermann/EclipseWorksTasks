using Microsoft.EntityFrameworkCore;
using EclipseWorksTasks.Models;

namespace EclipseWorksTasks.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Existem diversas formas de preparar a base. Preferi optar aqui 
            // pela simplicidade, focando na API em si.
            this.Database.ExecuteSqlRaw(File.ReadAllText("init-db.sql"));
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}