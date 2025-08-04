using DataModel.Constants;
using DataModel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Services.Services;
using System.Linq;
using ViewModel.Home;
using ViewModel.Login;
using ViewModel.Registration;
namespace UseNotesApplication.Controllers
{



    [Route("api/[controller]")]
    [ApiController]
    public class AccountApiController : ControllerBase
    {
        //Json Data
        string loginModelJson;
        LoginViewModel previousModel = null;
        private readonly UserServices _services;
        public AccountApiController(UserServices services)
        {
            _services = services;
        }

        //Session Strings
        
        //Sequence
        List<int> expectedSequence;
        public IActionResult SetExpectedSequence(Users user)
        {
            expectedSequence = user.Pictures.OrderBy(p => p.Sequence).Select(p => p.Id).ToList();

            if (expectedSequence == null)
            {
                return BadRequest(Constant.SessionError);
            }
            else return null;
        }

        //Register API Action
        [HttpPost("Register")]
        public IActionResult Register([FromForm] RegistrationViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_services.checkImagesCount(model))
            {
                return Conflict(Constant.imagesError);
            }
            if (_services.checkUserName(model.UserName))
            {
                return Conflict(Constant.userNameError);
            }
            if (_services.checkUserEmail(model.Email))
            {
                return Conflict(Constant.userEmailError);

            }
            
            try
            {
                _services.VmToDb(model);

                return Ok(new {message = "User registered successfully.", data = model });
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        //Login API Action
        [HttpPost("LoginUserName")]
        public IActionResult LoginUserName([FromForm] LoginViewModel model)
        {
            try
            {
                var user = _services.GetUserNameWithPictures(model.UserName);
                if (user == null)
                {
                    return BadRequest(new { error = Constant.userNameInvalid });
                }

                var randomImages = _services.CreateLoginImages();
                var currentPictures = _services.GetCurrentImages(user);
                var fullImages = currentPictures.Concat(randomImages)
                                                .OrderBy(x => Constant.GenerateCode())
                                                .ToList();

                SetExpectedSequence(user); // You may need to replace this with a session-independent solution if stateless
  
                var loginModel = _services.CreateLoginModel(model.UserName, fullImages);

                if (model.SelectedImageIds.Count != 5)
                {
                    return BadRequest(new { error = "Exactly 5 images must be selected" });
                }

                if (!expectedSequence.SequenceEqual(model.SelectedImageIds))
                {
                    return BadRequest(new { error = "Selected images do not match expected sequence", model });
                }

                return Ok(new { message = Constant.LoginSuccess ,data = loginModel});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }

        //Get Profile By UserName
        [HttpGet("GetProfile/{UserName}")]
        public IActionResult GetProfile([FromRoute] string UserName)
        {
            try
            {
                var user = _services.GetUser(UserName);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var model = _services.CreateProfileModel(user);
                return Ok(new
                {
                    message = "Profile fetched successfully.",
                    data = model
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the profile." });
            }
        }
        //Update Profile  Action
        [HttpPut("UpdateProfile/{UserName}")]
        public IActionResult UpdateProfile(string UserName, [FromBody] ProfileViewModel model)
        {
            var user = _services.GetUser(UserName);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Ensure email is unique across users (excluding current user)
            if (_services.checkUserEmail(model.Email) && !_services.checkUserName(model.UserName))
            {
                return BadRequest(new { field = "Email", message = Constant.userEmailError });
            }
            if(_services.checkUserName(model.UserName))
                return BadRequest(new { field = "UserName", message = Constant.userNameError });
            try
            {
                _services.UpdateProfile(user, model);
                return Ok(new { message = "Profile updated successfully." ,data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the profile." });
            }
        }
    }

}

