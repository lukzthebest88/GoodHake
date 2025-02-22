using Microsoft.EntityFrameworkCore;
using GoodHake.Models;

namespace GoodHake.Context
{
    public class GDDBContext : DbContext
    {
        public GDDBContext(DbContextOptions<GDDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<DailyIntake> DailyIntakes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase("CalorieDB");
        }
    }
}
