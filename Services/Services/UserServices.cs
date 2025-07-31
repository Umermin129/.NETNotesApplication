using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DataModel.Data;
using DataModel.Models;
using DataModel.Constants;
using ViewModel.Home;
using ViewModel.Login;
using ViewModel.Registration;
namespace Services.Services
{
    public class UserServices
    {
        public readonly AppDbContext _context;
        public FileConstants FileConstants=new FileConstants();
        public Users user;
        public readonly IWebHostBuilder _webHostEnvironment;
        public UserServices(AppDbContext context)
        {
            _context = context;
        }
        public UserServices()
        {
        }
        //VM To DB 
        public void VmToDb(RegistrationViewModel model)
        {
            user = model.Adapt<Users>();
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception($"{Constant.AddUserErr}:{e.Message}");
            }
            
        }
        //UserCredentials Methods
        public bool checkImagesCount(RegistrationViewModel model)
        {
            return model.Images.Count != 5 || model.Sequence.Count != 5;
        }
        public bool checkUserName(String userName)
        {
            try
            {
                return _context.Users.Any(u => u.UserName == userName);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.UserCheckErr}:{e.Message}");
            }
        }
        public bool checkUserEmail(String Email)
        {
            try
            {
                return _context.Users.Any(u => u.Email == Email);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.EmailCheckErr}:{e.Message}");
            }
        }
        //Folders Creation
        public void CreateUserFolder(RegistrationViewModel model)
        {
            try
            {
                FileConstants.SetUserFolderPath(model.UserName);
                Directory.CreateDirectory(FileConstants.userFolderPath);
                CreateImageURI(model);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.CreateUserFolderErr}:{e.Message}");
            }
            
        }
        public void CreateImageURI(RegistrationViewModel model)
        {
            for (int i = 0; i < 5; i++)
            {
                FileConstants.SetFormFile(model.Images[i]);
                FileConstants.SetSequence(model.Sequence[i]);
                FileConstants.GenerateFileName();
                FileConstants.GenerateFilePath();
                try
                {
                    FileConstants.CopyFileContent();
                }
                catch (Exception e)
                {
                    throw new Exception($"{Constant.FileCopyErr}:{e.Message}");
                }
                CreateImageDb();
            }
        }
        public void CreateImageDb()
        {
            var images = new UserPictures
            {
                ImageURI = FileConstants.SetImageURI(user.UserName),
                Sequence = FileConstants._sequence,
                UsersId = user.Id,
            };
            try
            {
                _context.UserPictures.Add(images);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.PictureDBErr}:{e.Message}");
            }
        }
        //Get Users
        public Users GetUserNameWithPictures(string UserName)
        {
            try
            {
                return _context.Users.Include(x => x.Pictures).FirstOrDefault(u => u.UserName == UserName);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.UserPicErr}:{e.Message}");
            }
        }
        public Users GetUser(string UserName)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.UserName == UserName);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.GetUserErr}:{e.Message}");
            }
        }
        public Users GetUserWithNotes(string UserName)
        {
            try
            {
                return _context.Users.Include(u => u.Notes).FirstOrDefault(u => u.UserName == UserName);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.GetUserErr}:{e.Message}");
            }
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
            return user.Adapt<ProfileViewModel>();
        }

        public void UpdateProfile(Users user, ProfileViewModel model)
        {
            model.Adapt(user);
            _context.SaveChanges();
        }

    }
}
