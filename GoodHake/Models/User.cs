using System.ComponentModel.DataAnnotations;

namespace GoodHake.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, MinLength(6)]
        public string PasswordHash { get; set; } // Passwort wird gehasht gespeichert

        [Required]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; } // "Männlich", "Weiblich"

        [Required]
        public double Weight { get; set; } // Gewicht in kg

        public int DailyCalorieGoal { get; set; } // Manuell einstellbare Tageskalorien
        public string Role { get; set; } = "User"; // Standardmäßig "User"
        public bool IsBanned { get; set; } = false; // Standardmäßig nicht gebannt
        public DailyIntake DailyIntake { get; set; }

    }
}
