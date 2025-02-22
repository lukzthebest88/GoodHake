using GoodHake.Context;
using GoodHake.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GoodHake.Controllers
{
    public class UserController : Controller
    {
        private readonly GDDBContext _context;

        public UserController(GDDBContext context)
        {
            _context = context;
        }

        public IActionResult Profile()
        {
            var user = _context.Users.FirstOrDefault() ?? new User();
            return View(user);
        }

        [HttpPost]
        public IActionResult Update(User user)
        {
            var existingUser = _context.Users.FirstOrDefault();
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Age = user.Age;
                existingUser.Gender = user.Gender;
                existingUser.Weight = user.Weight;
                existingUser.DailyCalorieGoal = user.DailyCalorieGoal;
            }
            else
            {
                _context.Users.Add(user);
            }
            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
    }
}
