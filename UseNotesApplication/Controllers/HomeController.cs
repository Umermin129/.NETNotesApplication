using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels;

namespace UseNotesApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var UserName = HttpContext.Session.GetString("UserName"); 
            var user  = _context.Users.Include(u=>u.Notes).FirstOrDefault(u=>u.UserName == UserName);

            if(user == null) {
                return RedirectToAction("Login","Account");
                }

            var model = new HomeViewModel
            {
                UserName = user.UserName,
                Name = user.Name,
                email = user.Email,
                TaskLists = user.Notes.Where(u => !u.IsDeleted).Select(n => new TaskEdit
                {
                    Title = n.Title,
                    Description = n.Description,
                    Status = n.Status,
                    LastUpdated = n.LastModifiedAt,
                }).ToList()
            };
            return View(model);
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
