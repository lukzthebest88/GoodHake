﻿using System.ComponentModel.DataAnnotations;

namespace GoodHake.Models
{
    public class DailyIntake
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public List<Meal> Meals { get; set; } = new List<Meal>();
        [Required]
        public string Name { get; set; } // Damit jede Tagesübersicht dem Benutzer gehört

        public int TotalCalories => Meals.Sum(m => m.Calories);
        public double TotalProtein => Meals.Sum(m => m.Protein);
        public double TotalFat => Meals.Sum(m => m.Fat);
        public double TotalCarbs => Meals.Sum(m => m.Carbs);
    }
}
