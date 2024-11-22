using Microsoft.EntityFrameworkCore;
using Uniqlol.Models;

namespace Uniqlol.DataAccess
{
    public class UniqloDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }


        public UniqloDbContext(DbContextOptions opt) :base(opt)
        {
            
        }


    }
}
