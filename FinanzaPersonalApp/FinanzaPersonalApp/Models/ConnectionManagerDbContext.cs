using Microsoft.EntityFrameworkCore;

namespace FinanzaPersonalApp.Models
{
    public class ConnectionManagerDbContext : DbContext
    {
        public ConnectionManagerDbContext(DbContextOptions<ConnectionManagerDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public DbSet<Models.Usuario> Usuarios { get; set; }
    }
}
