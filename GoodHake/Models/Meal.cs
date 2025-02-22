using System.ComponentModel.DataAnnotations;

namespace GoodHake.Models
{
    public class Meal
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Calories { get; set; }

        public double Protein { get; set; } // Eiweiß in g
        public double Fat { get; set; } // Fett in g
        public double Carbs { get; set; } // Kohlenhydrate in g
    }
}
