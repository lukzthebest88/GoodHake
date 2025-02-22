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

        /// <summary>
        /// Erstellt die InMemory-Datenbank und fügt Standardbenutzer hinzu
        /// </summary>
        public void Seed()
        {
            Database.EnsureDeleted(); // Datenbank zurücksetzen (optional für InMemory-DB)
            Database.EnsureCreated(); // Neue Datenbank erstellen

            if (!Users.Any(u => u.Name == "admin")) // Prüfen, ob Benutzer bereits existiert
            {
                var rootUser = new User
                {
                    Name = "admin",
                    PasswordHash = HashPassword("admin"),
                    Gender = "Männlich"// Sicherstellen, dass Passwort gehasht ist
                };

                Users.Add(rootUser);
                SaveChanges(); // Änderungen speichern
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
