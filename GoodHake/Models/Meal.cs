using System.ComponentModel.DataAnnotations;

namespace GoodHake.Models
{
    public class Meal
    {
        public int Id { get; set; }

        [Required]
        public string MealName { get; set; }

        public string Name { get; set; }

        [Required]
        public int Calories { get; set; }

        [Required]
        public double Protein { get; set; } // Eiweiß in g

        [Required]
        public double Fat { get; set; } // Fett in g

        [Required]
        public double Carbs { get; set; } // Kohlenhydrate in g
    }
}
