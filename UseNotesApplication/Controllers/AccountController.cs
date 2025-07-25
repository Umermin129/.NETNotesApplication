using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels;
using UseNotesApplication.Controllers;
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
                var rename = model.RenameImages[i];
                var fileExtension = Path.GetExtension(File.FileName);

                var fileName = $"{rename}{fileExtension}";
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
        //Login Logic
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LoginUserName(string UserName)
        {
            var user = _context.Users.Include(x => x.Pictures).FirstOrDefault(u => u.UserName == UserName);

            if (user == null)
            {
                ModelState.AddModelError("UserName", "Invalid UserName");
                return View("Login");
            }

            var randomImages = Enumerable.Range(1, 10)
                .Select(i => new LoginImage
                {
                    Id = i,
                    ImageURI = $"https://via.placeholder.com/100?text=Img{i}"
                })
                .ToList();
            var currentPictures = user.Pictures.Select(i => new LoginImage { Id = i.Id, ImageURI = i.ImageURI }).ToList();

            var fullImages = currentPictures.Concat(randomImages).OrderBy(x=> GenerateCode()).ToList();
            HttpContext.Session.SetString("ExpectedSequence", string.Join(",", user.Pictures.OrderBy(p => p.Sequence).Select(p => p.Id)));
            HttpContext.Session.SetString("UserName", UserName);
            var loginModel = new LoginViewModel
            {
                UserName = UserName,
                GridImages = fullImages
            };
            
            TempData["LoginModel"] = JsonSerializer.Serialize(loginModel);

            return View("LoginGrid", loginModel);
        }
        private int GenerateCode()
        {
            Random random = new Random();
            int number = random.Next(1, 11);
            Console.WriteLine(number);
            return number;
        }
        [HttpPost]
        [HttpPost]
        public IActionResult LoginConfirm(LoginViewModel model)
        {
            var loginModelJson = TempData["LoginModel"] as string;

            LoginViewModel previousModel = null;
            if (loginModelJson != null)
            {
                previousModel = JsonSerializer.Deserialize<LoginViewModel>(loginModelJson);
                TempData.Keep("LoginModel");
            }

            var expectedSequence = HttpContext.Session.GetString("ExpectedSequence");

            if (expectedSequence == null)
            {
                ModelState.AddModelError("", "Session Expired");
                return RedirectToAction("Login");
            }

            // Parse posted image ID string into a list
            var formSelectedIds = Request.Form["SelectedImageIds"].ToString();
            var selectedIds = formSelectedIds.Split(',').Select(id => int.Parse(id)).ToList();

            var expectedIds = expectedSequence.Split(',').Select(int.Parse).ToList();

            if (selectedIds.Count != 5)
            {
                ModelState.AddModelError("", "Please select exactly 5 images.");

                if (previousModel != null)
                {
                    model.GridImages = previousModel.GridImages;
                    model.UserName = previousModel.UserName;
                }

                return View("LoginGrid", model);
            }

            if (!expectedIds.SequenceEqual(selectedIds))
            {
                ModelState.AddModelError("", "Invalid Sequence");

                string userName = HttpContext.Session.GetString("UserName");
                var user = _context.Users.Include(u => u.Pictures).FirstOrDefault(u => u.UserName == userName);

                model.GridImages = user?.Pictures?.Select(i => new LoginImage
                {
                    Id = i.Id,
                    ImageURI = i.ImageURI
                }).ToList() ?? new List<LoginImage>();

                model.UserName = userName;

                return View("LoginGrid", model);
            }

            TempData["LoginSuccess"] = $"Welcome {HttpContext.Session.GetString("UserName")}";
            return RedirectToAction("Index");
        }
        //Home View Logic
        [HttpGet]
        public IActionResult Index()
        {

            return RedirectToAction("Index", "Home");
        }

        //[HttpGet]
      /*  public IActionResult UserGet()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var user = _context.Users.Include(u => u.Notes).FirstOrDefault(i => i.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("UserName", "User Not Found!");
                return View("Login");
            }
           
        }*/
    }
}
