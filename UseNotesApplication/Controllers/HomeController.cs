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
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var UserName = HttpContext.Session.GetString("UserName");
            if (UserName == null)
                return View();
            else
            {
                var user = _context.Users.Include(u => u.Notes).FirstOrDefault(u => u.UserName == UserName);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var model = new HomeViewModel
                {
                    UserName = user.UserName,
                    Name = user.Name,
                    email = user.Email,
                    TaskLists = user.Notes.Where(u => u.IsDeleted == false).Select(n => new TaskEdit
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Description = n.Description,
                        Status = n.Status,
                        LastUpdated = n.LastModifiedAt,
                    }).ToList()
                };
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TaskEdit model)
        {
            var UserName = HttpContext.Session.GetString("UserName");
            var user = _context.Users.FirstOrDefault(u => u.UserName == UserName);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var note = new Notes
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status ?? "Pending",
                UsersId = user.Id,
            };
            _context.Notes.Add(note);
            _context.SaveChanges();

            TempData["NoteSuccess"] = "Note Successfully Created";
            return RedirectToAction("Index");
        }
        [HttpPost]
        //Edit Notes 
        [HttpGet]
        public IActionResult GetNote(int id)
        {
            var UserName = HttpContext.Session.GetString("UserName");
            var user = _context.Users.Include(u => u.Notes).FirstOrDefault(u => u.UserName == UserName);

            var note = user?.Notes.FirstOrDefault(u => u.Id == id && !u.IsDeleted);

            if (note == null)
            {
                return RedirectToAction("index");
            }

            var model = new TaskEdit
            {
                Title = note.Title,
                Description = note.Description,
                Status = note.Status,
                LastUpdated = note.LastModifiedAt,
            };
            ViewBag.NoteId = id;
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(int id, TaskEdit model)
        {
            var UserName = HttpContext.Session.GetString("UserName");
            var note = _context.Notes.FirstOrDefault(u => u.Id == id && !u.IsDeleted && u.Users.UserName == UserName);

            if (note == null)
                return RedirectToAction("Index");

            var version = new NoteVersion
            {
                NotesId = note.Id,
                Title = note.Title,
                Description = note.Description,
                Status = note.Status,
                Timestamp = DateTime.UtcNow
            };
            _context.NoteVersions.Add(version);

            note.Title = model.Title;
            note.Description = model.Description;
            note.Status = model.Status ?? "Pending";
            note.LastModifiedAt = DateTime.UtcNow;
            _context.SaveChanges();

            TempData["NoteUpdated"] = "Notes Updated Successfully";
            return RedirectToAction("index");
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
        public IActionResult Delete(int Id)
        {
            var note = _context.Notes.FirstOrDefault(u => u.Id == Id);

            if (note == null)
                return RedirectToAction("index");

            note.IsDeleted = true;
            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
