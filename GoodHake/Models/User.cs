﻿using System.ComponentModel.DataAnnotations;

namespace GoodHake.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; } // "Männlich", "Weiblich"

        [Required]
        public double Weight { get; set; } // Gewicht in kg

        public int DailyCalorieGoal { get; set; } // Manuell einstellbare Tageskalorien
    }
}
