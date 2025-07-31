using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;
using UseNotesApplication.Data;
using UseNotesApplication.Constants;
using UseNotesApplication.Models;
using UseNotesApplication.Services;
using UseNotesApplication.ViewModels;
using UseNotesApplication.ViewModels.Home;
using UseNotesApplication.ViewModels.Login;
using UseNotesApplication.ViewModels.Registration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
namespace UseNotesApplication.Controllers
{
    public class AccountController : Controller
    {
        private UserServices _services;
       
        public AccountController(UserServices userServices)
        {
            _services = userServices;
        }
        //Check Methods
        private void ReCreateGridImages(LoginViewModel model)
        {
            ModelState.AddModelError("", Constant.imagesError);

            if (previousModel != null)
            {
                model.GridImages = previousModel.GridImages;
                model.UserName = previousModel.UserName;
            }
        }
        private void RecreateUserImages(LoginViewModel  model)
        {
            ModelState.AddModelError("", Constant.InvalidSequence);

            string userName = SessionGetUserName();
            var user = _services.GetUserNameWithPictures(userName);

            model.GridImages = user?.Pictures?.Select(i => new LoginImage
            {
                Id = i.Id,
                ImageURI = i.ImageURI
            }).ToList() ?? new List<LoginImage>();

            model.UserName = userName;
        }
        //Session Strings
        private void SessionSetExpectedSequence(Users user)
        {
            HttpContext.Session.SetString(Constant.ExpectedSequence, string.Join(",", user.Pictures.OrderBy(p => p.Sequence).Select(p => p.Id)));
        }
        private string SessionGetExpectedSequence()
        {
            return HttpContext.Session.GetString(Constant.ExpectedSequence);
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
            TempData["LogOutSuccess"] = Constant.LogoutSuccess;
        }
        //ModelState Errors
        private void ModelError(string key,string error)
        {
            ModelState.AddModelError(key, error);
        }

        //TempData methods
        public void RegisterSuccess()
        {
            TempData["Success"] = Constant.RegisterSuccess;
        }
        public void LoginModelSerialize(LoginViewModel loginModel)
        {
            TempData["LoginModel"] = JsonSerializer.Serialize(loginModel);
        }
        public void UserUpdateSuccess()
        {
            TempData["ProfileUpdated"] = Constant.ProfileUpdate;
        }
        //temData Success
        private void LoginSuccess()
        {
            TempData["LoginSuccess"] = Constant.LoginSuccess;
        }
        //TempData Error
        private void RegisterError(Exception error)
        {
            TempData["RegistrationError"] = error.Message;
        }
        private void LoginError(Exception error)
        {
            TempData["LoginError"] = error.Message;
        }
        private void GetProfileError(Exception error)
        {
            TempData["GetProfile"] = error.Message;

        }
        private void UpdateProfileError(Exception error)
        {
            TempData["UpdateProfileError"] = error.Message;
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
                ModelState.AddModelError("", Constant.SessionError);
                return RedirectToAction(Constant.LoginAction);
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(Constant.RegisterView, model);
                }
                if (_services.checkImagesCount(model))
                {
                    ModelError(string.Empty, Constant.imagesError);
                    return View(Constant.RegisterView, model);
                }
                if (_services.checkUserName(model.UserName))
                {
                    ModelError(Constant.errUserName, Constant.userNameError);
                    return View(Constant.RegisterView, model);
                }
                if (_services.checkUserEmail(model.Email))
                {
                    ModelError(Constant.errEmail, Constant.userEmailError);
                    return View(Constant.RegisterView, model);
                }
                _services.VmToDb(model);

                _services.CreateUserFolder(model);

                RegisterSuccess();
                return RedirectToAction(Constant.LoginAction);
            }

            catch (Exception ex)
            {
                RegisterError(ex);
                return View(Constant.RegisterView, model);
            }
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
            try
            {
                var user = _services.GetUserNameWithPictures(UserName);
                if (user == null)
                {
                    ModelState.AddModelError("UserName", Constant.userNameInvalid);
                    return View(Constant.LoginView);
                }

                var randomImages = _services.CreateLoginImages();
                var currentPictures = _services.GetCurrentImages(user);

                var fullImages = currentPictures.Concat(randomImages).OrderBy(x => Constant.GenerateCode()).ToList();
                SessionSetExpectedSequence(user);
                SessionSetUserName(UserName);
                var loginModel = _services.CreateLoginModel(UserName, fullImages);
                LoginModelSerialize(loginModel);
                return View(Constant.LoginGridView, loginModel);
            }
            catch(Exception ex) 
            {
                LoginError(ex);
                return View(Constant.LoginView);
            }
        }
        
        [HttpPost]
        public IActionResult LoginConfirm(LoginViewModel model)
        {
            try
            {

                SetLoginModelJson();
                SetPreviousModel();
                SetExpectedSequence();

                // Parse posted image ID string into a list
                ConvertFormIdToSelectedId();

                var expectedIds = expectedSequence.Split(',').Select(int.Parse).ToList();
                if (selectedIds.Count != 5)
                {
                    ReCreateGridImages(model);
                    return View("LoginGrid", model);
                }

                if (!expectedIds.SequenceEqual(selectedIds))
                {
                    RecreateUserImages(model);
                    return View("LoginGrid", model);
                }
                LoginSuccess();
                return RedirectToAction("Index");
            }
            catch (Exception ex) {
                LoginError(ex);
                return View("Login");
            }
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
            try
            {
                var userName = HttpContext.Session.GetString("UserName");
                var user = _services.GetUser(userName);
                if (user == null) return RedirectToAction("Login");

                var model = _services.CreateProfileModel(user);

                return View(model);
            }
            catch (Exception ex) {
                GetProfileError(ex);
                return View("index");
            }
        }

        [HttpPost]
        public IActionResult UpdateProfile(ProfileViewModel model)
        {
            var UserName = SessionGetUserName();
            var user = _services.GetUser(UserName);
            if (user == null) return RedirectToAction(Constant.IndexAction, "Home");

            if (_services.checkUserEmail(model.Email) && !_services.checkUserName(model.UserName))
            {
                ModelError(Constant.errEmail,Constant.userEmailError);
                return View(Constant.GetProfileView, model);
            }
            try
            {
                _services.UpdateProfile(user, model);
                UserUpdateSuccess();
                return RedirectToAction(Constant.IndexAction);
            }
            catch (Exception ex) {
                UpdateProfileError(ex);
                return RedirectToAction(Constant.IndexAction, "Home");
            }
        }
        [HttpGet]
        public IActionResult LogOut()
        {
            SessionClear();
            return RedirectToAction(Constant.LoginAction);
        }
    }
}