using System;
using System.Diagnostics;
using GoodHake.Context;
using GoodHake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodHake.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GDDBContext _context;

        public HomeController(ILogger<HomeController> logger, GDDBContext context)
        {
            _logger = logger;
            _context = context;

        }
        public IActionResult Index()
        {
            var today = System.DateTime.Today;

            // Tagesübersicht mit Mahlzeiten laden
            var dailyIntake = _context.DailyIntakes
                .Include(d => d.Meals) // Sicherstellen, dass Mahlzeiten geladen werden!
                .FirstOrDefault(d => d.Date.Date == today);

            if (dailyIntake == null)
            {
                dailyIntake = new DailyIntake { Date = today, Meals = new List<Meal>() };
                _context.DailyIntakes.Add(dailyIntake);
                _context.SaveChanges();
            }

            return View(dailyIntake);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
