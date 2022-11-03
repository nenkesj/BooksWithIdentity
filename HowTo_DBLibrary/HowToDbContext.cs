using Microsoft.EntityFrameworkCore;

namespace HowTo_DBLibrary
{
    public class HowToDbContext : DbContext
    {
        //Add a default constructor if scaffolding is needed
        public HowToDbContext() { }
        //Add the complex constructor for allowing Dependency Injection
        public HowToDbContext(DbContextOptions options)
        : base(options)
        {
            //intentionally empty.
        }
    }
}