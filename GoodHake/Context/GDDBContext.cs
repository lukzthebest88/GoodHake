using Microsoft.EntityFrameworkCore;
using GoodHake.Models;
using System.Text;
using System.Security.Cryptography;

namespace GoodHake.Context
{
    public class GDDBContext : DbContext
    {
        public GDDBContext(DbContextOptions<GDDBContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<DailyIntake> DailyIntakes { get; set; }
        public DbSet<Meal> Meals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public void Seed()
        {
            Database.EnsureCreated(); // Neue Datenbank erstellen, falls nicht vorhanden

            var adminUser = Users.FirstOrDefault(u => u.Name == "admin");

            if (adminUser == null) // Prüfen, ob "admin" existiert
            {
                adminUser = new User
                {
                    Name = "admin",
                    PasswordHash = HashPassword("admin"),
                    Gender = "Männlich",
                    Role = "Admin" // Admin-Rolle setzen
                };

                Users.Add(adminUser);
                SaveChanges();
            }
            else
            {
                adminUser.Role = "Admin"; // Falls er existiert, sicherstellen, dass er Admin ist
                SaveChanges();
            }
        }

        /// <summary>
        /// Hash-Funktion für Passwörter (SHA256)
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
