using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels;

namespace UseNotesApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        //Register Module Logic
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid || model.Images.Count != 5 || model.Sequence.Count != 5)
            {
                ModelState.AddModelError("", "Please Upload exactly 5 images and Sequence!");
                return View(model);
            }
            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "UserName already Exists!");
                return View(model);
            }
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "User with this email already Exists!");
            }
            var user = new Users
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            String userFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UserImages", user.UserName);
            Directory.CreateDirectory(userFolder);

            for (int i = 0; i < 5; i++)
            {
                var File = model.Images[i];
                var sequence = model.Sequence[i];

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(File.FileName)}";
                var filePath = Path.Combine(userFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }
                var images = new UserPictures
                {
                    ImageURI = Path.Combine("UserImages", user.UserName, fileName).Replace("\\", "/"),
                    Sequence = sequence,
                    UsersId = user.Id,

                };
                _context.UserPictures.Add(images);
                _context.SaveChanges();
            }
            TempData["Success"] = "Registered Successfully";
            return RedirectToAction("Login");
        }
        
    }
}
