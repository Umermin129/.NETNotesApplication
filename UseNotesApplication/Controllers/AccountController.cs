using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
using UseNotesApplication.Controllers;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.Services;
using UseNotesApplication.ViewModels;
using UseNotesApplication.ViewModels.Home;
using UseNotesApplication.ViewModels.Login;
using UseNotesApplication.ViewModels.Registration;
namespace UseNotesApplication.Controllers
{
    public class AccountController : Controller
    {
        private UserServices _services;
       
        public AccountController(UserServices userServices)
        {
            _services = userServices;
        }

        //Session Strings
        private void SessionSetExpectedSequence(Users user)
        {
            HttpContext.Session.SetString(Constants.ExpectedSequence, string.Join(",", user.Pictures.OrderBy(p => p.Sequence).Select(p => p.Id)));
        }
        private string SessionGetExpectedSequence()
        {
            return HttpContext.Session.GetString(Constants.ExpectedSequence);
        }
        private void SessionSetUserName(string UserName)
        {
            HttpContext.Session.SetString("UserName", UserName);
        }
        private string SessionGetUserName()
        {
            return HttpContext.Session.GetString("UserName");
        }
        private void SessionClear()
        {
            HttpContext.Session.Clear();
            TempData["LogOutSuccess"] = Constants.LogoutSuccess;
        }
        //ModelState Errors
        private void ModelError(string key,string error)
        {
            ModelState.AddModelError(key, error);
        }

        //TempData methods
        public void RegisterSuccess()
        {
            TempData["Success"] = Constants.RegisterSuccess;
        }
        public void LoginModelSerialize(LoginViewModel loginModel)
        {
            TempData["LoginModel"] = JsonSerializer.Serialize(loginModel);
        }
        public void UserUpdateSuccess()
        {
            TempData["ProfileUpdated"] = Constants.ProfileUpdate;
        }
        //Json Data
        string loginModelJson;
        LoginViewModel previousModel = null;
        private void SetPreviousModel()
        {
            if (GetLoginModelJson() != null)
            {
                previousModel = JsonSerializer.Deserialize<LoginViewModel>(GetLoginModelJson());
                TempData.Keep("LoginModel");
            }
        }
        private void SetLoginModelJson()
        {
            var loginModelJson = TempData["LoginModel"] as string;
        }
        private string GetLoginModelJson()
        {
            return loginModelJson;
        }
        //Sequence
        string expectedSequence;
        public IActionResult SetExpectedSequence()
        {
            expectedSequence = SessionGetExpectedSequence();

            if (expectedSequence == null)
            {
                ModelState.AddModelError("", Constants.SessionError);
                return RedirectToAction(Constants.LoginAction);
            }
            else return null;
        }
        //Form Data
        string formSelectedIds;
        List<int> selectedIds;
        public void ConvertFormIdToSelectedId()
        {
            formSelectedIds = Request.Form["SelectedImageIds"].ToString();
            selectedIds = formSelectedIds.Split(',').Select(id => int.Parse(id)).ToList();
        }
        //Register Module Logic
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ConfirmRegister(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(Constants.RegisterView,model);
            }
            if (_services.checkImagesCount(model))
            {
                ModelError(string.Empty,Constants.imagesError);
                return View(Constants.RegisterView, model);
            }
            if (_services.checkUserName(model.UserName))
            {
                ModelError(Constants.errUserName, Constants.userNameError);
                return View(Constants.RegisterView,model);
            }
            if (_services.checkUserEmail(model.Email))
            {
                ModelError(Constants.errEmail,Constants.userEmailError);
                return View(Constants.RegisterView,model);
            }
            _services.VmToDb(model);

            _services.CreateUserFolder(model);
            
            RegisterSuccess();
            return RedirectToAction(Constants.LoginAction);
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
            var user = _services.GetUserNameWithPictures(UserName);
            if (user == null)
            {
                ModelState.AddModelError("UserName", Constants.userNameInvalid);
                return View(Constants.LoginView);
            }

            var randomImages = _services.CreateLoginImages();
            var currentPictures = _services.GetCurrentImages(user);

            var fullImages = currentPictures.Concat(randomImages).OrderBy(x => Constants.GenerateCode()).ToList();
            SessionSetExpectedSequence(user);
            SessionSetUserName(UserName);
            var loginModel = _services.CreateLoginModel(UserName, fullImages);
            LoginModelSerialize(loginModel);
            return View(Constants.LoginGridView, loginModel);
        }
        
        [HttpPost]
        public IActionResult LoginConfirm(LoginViewModel model)
        {
            SetLoginModelJson();
            SetPreviousModel();
            SetExpectedSequence();

            // Parse posted image ID string into a list
            ConvertFormIdToSelectedId();

            var expectedIds = expectedSequence.Split(',').Select(int.Parse).ToList();

            if (selectedIds.Count != 5)
            {
                ModelState.AddModelError("", Constants.imagesError);

                if (previousModel != null)
                {
                    model.GridImages = previousModel.GridImages;
                    model.UserName = previousModel.UserName;
                }

                return View("LoginGrid", model);
            }

            if (!expectedIds.SequenceEqual(selectedIds))
            {
                ModelState.AddModelError("", Constants.InvalidSequence);

                string userName = SessionGetUserName();
                var user = _services.GetUserNameWithPictures(userName);

                model.GridImages = user?.Pictures?.Select(i => new LoginImage
                {
                    Id = i.Id,
                    ImageURI = i.ImageURI
                }).ToList() ?? new List<LoginImage>();

                model.UserName = userName;

                return View("LoginGrid", model);
            }

            TempData["LoginSuccess"] = Constants.LoginSuccess;
            return RedirectToAction("Index");
        }
        //Home View Logic
        [HttpGet]
        public IActionResult Index()
        {

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var user = _services.GetUser(userName);
            if (user == null) return RedirectToAction("Login");

            var model = _services.CreateProfileModel(user);

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileViewModel model)
        {
            var UserName = SessionGetUserName();
            var user = _services.GetUser(UserName);
            if (user == null) return RedirectToAction(Constants.IndexAction, "Home");

            if (_services.checkUserEmail(model.Email) && !_services.checkUserName(model.UserName))
            {
                ModelError(Constants.errEmail,Constants.userEmailError);
                return View(Constants.GetProfileView, model);
            }
            _services.UpdateProfile(user, model);
            UserUpdateSuccess();
            return RedirectToAction(Constants.IndexAction);
        }
        [HttpGet]
        public IActionResult LogOut()
        {
            SessionClear();
            return RedirectToAction(Constants.LoginAction);
        }
    }
}