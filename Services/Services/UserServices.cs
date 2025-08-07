using DataModel.Constants;
using DataModel.Data;
using DataModel.Models;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ViewModel.Home;
using ViewModel.Login;
using ViewModel.Registration;
namespace Services.Services
{
    public class UserServices
    {
        public readonly AppDbContext _context;
        public Users user;
        public readonly IWebHostBuilder _webHostEnvironment;
        public UserServices(AppDbContext context)
        {
            _context = context;
        }
        public UserServices()
        {
        }
        public bool CheckUniqueness(Response response,RegistrationViewModel model)
        {
            bool flag = false;
            
            if (checkUserName(model.UserName))
            {
                response.StatusCode = 409;
                response.Message = Constant.userEmailError;
                flag = true;
            }
            if (checkUserEmail(model.Email))
            {
                response.StatusCode = 409;
                response.Message = Constant.userEmailError;
                flag = true;
            }
            response.ModelObject = model;
            return flag;
        }
        public void RegisterSuccess(Response Response, RegistrationViewModel model)
        {

            Response.StatusCode = 200;
            Response.Message = Constant.RegisterSuccess;
            Response.ModelObject = model;
        }
        //VM To DB 
        public Response VmToDb(RegistrationViewModel model)
        {
            Response response = new Response();
            
            if(CheckUniqueness(response, model))
            {
                return response;
            }
            else
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        user = model.Adapt<Users>();
                        _context.Users.Add(user);
                        _context.SaveChanges();
                        transaction.CreateSavepoint("UserSaved");
                        CreateUserFolder(model);
                        transaction.Commit();
                        RegisterSuccess(response,model);
                        return response;
                    }
                    catch (Exception e)
                    { 
                        transaction.RollbackToSavepoint("UserSaved");
                        throw new Exception($"{Constant.AddUserErr}:{e.Message}");
                    }
                }
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
                FileConfig.SetUserFolderPath(model.UserName);
                Directory.CreateDirectory(FileConfig.UserFolderPath);
                CreateImageURI(model);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.CreateUserFolderErr}:{e.Message}");
            }

        }


        public void CreateImageURI(RegistrationViewModel model)
        {
            List<FileConfig> fileConfigs = new List<FileConfig>();

            for (int i = 0; i < 5; i++)
            {
                FileConfig config = new FileConfig(model.Images[i], model.Sequence[i]);

                config.GenerateFilePath();
                config.GenerateReferencePath(user.UserName);
                try
                {
                    config.CopyFileContent();
                }
                catch (Exception e)
                {
                    throw new Exception($"{Constant.FileCopyErr}:{e.Message}");
                }
                fileConfigs.Add(config);
            }
            CreateImageDb(fileConfigs);
        }
        public void CreateImageDb(List<FileConfig> fileConfigs)
        {

            foreach (FileConfig file in fileConfigs)
            {
                var images = new UserPictures
                {
                    ImageURI = file.referncePath,
                    Sequence = file.Sequence,
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
                    ImageURI = $"UserImages/Image{i}.jpg"
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
