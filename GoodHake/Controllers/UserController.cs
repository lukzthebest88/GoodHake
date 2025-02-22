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
                return RedirectToAction("Login"); // Falls User nicht gefunden wird, zum Login schicken
            }

            return View(user);
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
        public IActionResult Login(string name, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Name == name);

            if (user == null || user.PasswordHash != HashPassword(password))
            {
                ModelState.AddModelError("", "Ungültiger Name oder Passwort.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name ?? "")
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
