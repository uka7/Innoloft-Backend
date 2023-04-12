using Microsoft.EntityFrameworkCore;

namespace Innoloft.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        

    }
}