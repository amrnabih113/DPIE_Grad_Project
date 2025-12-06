using System.Diagnostics;
using Bookify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitContact(string name, string email, string phone, string subject, string message)
        {
            try 
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || 
                    string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
                {
                    return Json(new { success = false, message = "Please fill in all required fields." });
                }

                // Email validation
                if (!IsValidEmail(email))
                {
                    return Json(new { success = false, message = "Please enter a valid email address." });
                }

                // Log the contact form submission
                _logger.LogInformation("Contact form submitted - Name: {Name}, Email: {Email}, Subject: {Subject}", 
                    name, email, subject);

                // In a real application, you would:
                // 1. Save to database
                // 2. Send email notification
                // 3. Send auto-reply to customer

                return Json(new { success = true, message = "Thank you for your message! We'll get back to you soon." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission");
                return Json(new { success = false, message = "An error occurred while processing your request. Please try again." });
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
