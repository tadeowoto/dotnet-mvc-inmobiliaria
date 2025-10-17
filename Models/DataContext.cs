using Microsoft.EntityFrameworkCore;


namespace inmobiliaria.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
    }
}