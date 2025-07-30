using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UseNotesApplication.Controllers;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels;
using UseNotesApplication.ViewModels.Login;
using UseNotesApplication.ViewModels.Registration;
namespace UseNotesApplication.Services
{
    public class UserServices
    {
        public readonly AppDbContext _context;
        public Users user;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public UserServices(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public UserServices()
        {
        }
        //Handle Errors
        public void VmToDb(RegistrationViewModel model)
        {
            user = new Users
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        //UserCredentials Methods
        public bool checkImagesCount(RegistrationViewModel model)
        {
            return model.Images.Count != 5 || model.Sequence.Count != 5;
        }
        public bool checkUserName(String userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }
        public bool checkUserEmail(String Email)
        {
            return _context.Users.Any(u => u.Email == Email);
        }
        //Folders Creation
        public void CreateUserFolder(RegistrationViewModel model)
        {
            String userFolder = Path.Combine(_webHostEnvironment.WebRootPath, "UserImages", user.UserName);
            Directory.CreateDirectory(userFolder);
            CreateImageURI(model, userFolder);
        }
        public void CreateImageURI(RegistrationViewModel model, String userFolder)
        {
            for (int i = 0; i < 5; i++)
            {
                var File = model.Images[i];
                var sequence = model.Sequence[i];
                var fileExtension = Path.GetExtension(File.FileName);

                var fileName = $"{Guid.NewGuid()}_{sequence}{fileExtension}";
                var filePath = Path.Combine(userFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }
                CreateImageDb(fileName, sequence);
            }
        }
        public void CreateImageDb(string fileName, int sequence)
        {
            var images = new UserPictures
            {
                ImageURI = Path.Combine("UserImages", user.UserName, fileName).Replace("\\", "/"),
                Sequence = sequence,
                UsersId = user.Id,
            };
            _context.UserPictures.Add(images);
            _context.SaveChanges();
        }
        //Get Users
        public Users GetUserNameWithPictures(string UserName)
        {
            return _context.Users.Include(x => x.Pictures).FirstOrDefault(u => u.UserName == UserName);
        }
        public Users GetUser(string UserName)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == UserName);
        }
        public Users GetUserWithNotes(string UserName)
        {
            return _context.Users.Include(u => u.Notes).FirstOrDefault(u => u.UserName == UserName);
        }
        //Images Grid Create
        public List<LoginImage> CreateLoginImages()
        {
            return Enumerable.Range(1, 10)
                .Select(i => new LoginImage
                {
                    Id = i,
                    ImageURI = $"/UserImages/Image{i}.jpg"
                })
                .ToList();
        }
        public List<LoginImage> GetCurrentImages(Users user)
        {
            return user.Pictures.Select(i => new LoginImage { Id = i.Id, ImageURI = i.ImageURI }).ToList();
        }
        //ViewModel Objects Creation
        public LoginViewModel CreateLoginModel(string UserName, List<LoginImage> fullImages)
        {
            var loginModel = new LoginViewModel
            {
                UserName = UserName,
                GridImages = fullImages
            };
            return loginModel;
        }

        public ProfileViewModel CreateProfileModel(Users user)
        {
            return new ProfileViewModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email
            };
        }

        public void UpdateProfile(Users user, ProfileViewModel model)
        {
            user.Email = model.Email;
            user.Name = model.Name;
        }

    }
}
