using Microsoft.EntityFrameworkCore;
using Uniqlol.Models;

namespace Uniqlol.DataAccess
{
    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }


        public UniqloDbContext(DbContextOptions opt) :base(opt)
        {
            
        }


    }
}
