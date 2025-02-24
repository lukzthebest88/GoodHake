using GoodHake.Context;
using GoodHake.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GoodHake.Controllers
{
    [Authorize]
    public class MealController : Controller
    {
        private readonly GDDBContext _context;

        public MealController(GDDBContext context)
        {
            _context = context;
        }

        public IActionResult List()
        {
            var userName = User.Identity.Name; //  Eingeloggten Benutzer abrufen

            if (string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("Login", "Account"); // Falls nicht eingeloggt, weiterleiten
            }

            var meals = _context.Meals.Where(m => m.Name == userName).ToList();
            return View(meals);
        }


        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Meal meal)
        {
            var userName = User.Identity.Name; // Eingeloggten Benutzer abrufen

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(); // Falls nicht eingeloggt, Zugriff verweigern
            }

            if (ModelState.IsValid)
            {
                 meal.Name = userName; // Mahlzeit dem Benutzer zuweisen
                _context.Meals.Add(meal);
                _context.SaveChanges();

                // Tagesübersicht für den aktuellen Benutzer abrufen oder erstellen
                var today = DateTime.Today;
                var dailyIntake = _context.DailyIntakes
                    .FirstOrDefault(d => d.Date == today && d.Name == userName);

                if (dailyIntake == null)
                {
                    dailyIntake = new DailyIntake { Date = today, Name = userName, Meals = new List<Meal>() };
                    _context.DailyIntakes.Add(dailyIntake);
                }

                dailyIntake.Meals.Add(meal);
                _context.SaveChanges();

                return RedirectToAction("List");
            }
            return View(meal);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var meal = _context.Meals.Find(id);
            if (meal == null)
            {
                return NotFound();
            }

            _context.Meals.Remove(meal);
            _context.SaveChanges();

            return RedirectToAction("List", "Meal"); // Explizit zum "Index" der "MealController"
        }

    }
}
