using GoodHake.Context;
using GoodHake.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GoodHake.Controllers
{
    public class UserController : Controller
    {
        private readonly GDDBContext _context;

        public UserController(GDDBContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult List()
        {
            var users = _context.Users.ToList();
            return Json(users); // Alle Benutzer als JSON zurückgeben
        }

        [Authorize]
        public IActionResult UserList()
        {
            if (!User.Claims.Any(c => c.Type == "role" && c.Value == "Admin"))
            {
                return Forbid(); // Kein Zugriff für Nicht-Admins
            }

            var users = _context.Users.ToList();
            return View(users);
        }

        [Authorize]
        public IActionResult Ban(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsBanned = true;
            _context.SaveChanges();
            return RedirectToAction("UserList");
        }

        [Authorize]
        public IActionResult Unban(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsBanned = false;
            _context.SaveChanges();
            return RedirectToAction("UserList");
        }

        [Authorize]
        public IActionResult Stats(int id)
        {
            var user = _context.Users
                .Where(u => u.Id == id)
                .Select(u => new User
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age,
                    Gender = u.Gender,
                    Weight = u.Weight,
                    DailyCalorieGoal = u.DailyCalorieGoal,
                    Role = u.Role,
                    IsBanned = u.IsBanned,
                    DailyIntake = _context.DailyIntakes
                        .Where(d => d.Name == u.Name && d.Date.Date == DateTime.Today)
                        .Select(d => new DailyIntake
                        {
                            Date = d.Date,
                            Meals = _context.Meals.Where(m => d.Meals.Contains(m)).ToList(),
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }



        public IActionResult Register()
        {
            return View();
        }

        [Authorize]
        public IActionResult Profile()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Name == username);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Update(User updatedUser)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Name == username);

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Werte aktualisieren
            user.Name = updatedUser.Name;
            user.Age = updatedUser.Age;
            user.Gender = updatedUser.Gender;
            user.Weight = updatedUser.Weight;
            user.DailyCalorieGoal = updatedUser.DailyCalorieGoal;
            // Speichern versuchen
            try
            {
                _context.SaveChanges();
                Console.WriteLine("Benutzerprofil erfolgreich gespeichert!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern: {ex.Message}");
            }
            if (!ModelState.IsValid)
            {
                return View("Profile", updatedUser);
            }
            return RedirectToAction("Profile");
        }


        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Name == user.Name))
            {
                ModelState.AddModelError("Name", "Der Name ist bereits registriert.");
                return View();
            }

            if (string.IsNullOrEmpty(user.Name)) // Sicherstellen, dass Name gesetzt ist
            {
                ModelState.AddModelError("Name", "Bitte gib einen Benutzernamen ein.");
                return View();
            }

            user.PasswordHash = HashPassword(user.PasswordHash); // Passwort-Hashing

            _context.Users.Add(user); // Benutzer speichern
            _context.SaveChanges();    // Änderungen in die DB schreiben

            return RedirectToAction("Login");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Name == user.Name);

            if (existingUser == null || existingUser.PasswordHash != HashPassword(user.PasswordHash))
            {
                ModelState.AddModelError("", "Ungültiger Name oder Passwort.");
                return View(user);
            }

            if (existingUser.IsBanned)
            {
                ModelState.AddModelError("", "Dieser Benutzer wurde gesperrt.");
                return View(user);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.Name ?? ""),
                new Claim("role", existingUser.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
