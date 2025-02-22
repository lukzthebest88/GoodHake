using GoodHake.Context;
using GoodHake.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GoodHake.Controllers
{
    public class MealController : Controller
    {
        private readonly GDDBContext _context;

        public MealController(GDDBContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var meals = _context.Meals.ToList();
            return View(meals);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Meal meal)
        {
            if (ModelState.IsValid)
            {
                // Mahlzeit speichern
                _context.Meals.Add(meal);
                _context.SaveChanges();

                // 🔥 WICHTIG: Tagesübersicht für heute abrufen oder neu erstellen
                var today = System.DateTime.Today;
                var dailyIntake = _context.DailyIntakes
                    .FirstOrDefault(d => d.Date.Date == today);

                if (dailyIntake == null)
                {
                    dailyIntake = new DailyIntake { Date = today, Meals = new List<Meal>() };
                    _context.DailyIntakes.Add(dailyIntake);
                }

                // Mahlzeit zur Tagesübersicht hinzufügen
                dailyIntake.Meals.Add(meal);
                _context.SaveChanges();

                return RedirectToAction("List");
            }
            return View(meal);
        }
    }
}
