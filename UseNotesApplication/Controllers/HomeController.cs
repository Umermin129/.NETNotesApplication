using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using DataModel.Constants;
using DataModel.Models;
using Services.Services;
using ViewModel.Notes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UseNotesApplication.Controllers
{
    public class HomeController : Controller
    {
        //Session Strings
        private void SessionSetUserName(string UserName)
        {
            HttpContext.Session.SetString("UserName", UserName);
        }
        private string SessionGetUserName()
        {
            return HttpContext.Session.GetString("UserName");
        }
        //TempData Success
        private void NotesCreateSuccess()
        {
            TempData["NoteSuccess"] = Constant.NotesSuccess;
        }
        private void NotesUpdateSuccess()
        {
            TempData["NoteUpdated"] = Constant.NoteUpdate;
        }
        //TempData Error
        private void NotesCreateError(Exception error)
        {
            TempData["NotesCreateError"] = error.Message;
        }
        private void UserDataLoadError(Exception error)
        {
            TempData["UserDataError"] = error.Message;
        }
        private void GetNoteError(Exception error)
        {
            TempData["GetNoteError"] = error.Message;
        }
        private void EditNoteError(Exception error)
        {
            TempData["EditNoteError"] = error.Message;
        }


        private readonly ILogger<HomeController> _logger;

        UserServices _userService;
        NotesServices _noteService;
        public HomeController(ILogger<HomeController> logger, UserServices userServices, NotesServices notesServices)
        {
            _userService = userServices;
            _noteService = notesServices;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var UserName = SessionGetUserName();
                if (UserName == null)
                    return View();
                else
                {
                    var userData = _userService.GetUserWithNotes(UserName);

                    if (userData == null)
                    {
                        return RedirectToAction(Constant.LoginAction, "Account");
                    }

                    var model = _noteService.CreateHomeViewModel(userData);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                UserDataLoadError(ex);
                return RedirectToAction(Constant.LoginAction, "Account");
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(Constant.CreateRoute);
        }
        [HttpPost]
        public IActionResult CreateNote(TaskEditViewModel model)
        {
            try
            {
                var UserName = SessionGetUserName();
                var userData = _userService.GetUser(UserName);

                if (userData == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                _noteService.CreateNotes(userData, model);
            }catch(Exception ex)
            {
                NotesCreateError(ex);
            }

            NotesCreateSuccess();
            return RedirectToAction("Index");
        }
        [HttpPost]
        //Edit Notes 
        [HttpGet]
        public IActionResult GetNote(int id)
        {
            try
            {
                var UserName = SessionGetUserName();
                var userData = _userService.GetUserWithNotes(UserName);

                var noteData = _noteService.GetNote(userData, id);
                if (noteData == null)
                {
                    return RedirectToAction(Constant.IndexAction);
                }

                var noteViewModel = _noteService.CreateViewModel(noteData);
                ViewBag.NoteId = id;
                return View(Constant.GetNoteRoute, noteViewModel);
            }
            catch(Exception ex) 
            {
                GetNoteError(ex);
                return RedirectToAction(Constant.IndexAction);
            }
        }
        [HttpPost]
        public IActionResult Edit(int id, TaskEditViewModel model)
        {
            try
            {
                var UserName = SessionGetUserName();
                var userData = _userService.GetUserWithNotes(UserName);
                var noteData = _noteService.GetNote(userData, id);

                if (noteData == null)
                    return RedirectToAction(Constant.IndexAction);

                _noteService.CreateNoteVersion(noteData);
                _noteService.UpdateNote(noteData, model);
                NotesUpdateSuccess();
                return RedirectToAction(Constant.IndexAction);
            }
            catch (Exception ex) { 
               EditNoteError(ex);
                return RedirectToAction(Constant.IndexAction);
            }
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
            var note = _noteService.GetNote(Id);

            if (note == null)
                return RedirectToAction(Constant.IndexAction);

            _noteService.DeleteNote(note);
            return RedirectToAction(Constant.IndexAction);
        }
    }
}
