using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels.Notes;
using UseNotesApplication.ViewModels.Home;
using UseNotesApplication.Services;

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
            TempData["NoteSuccess"] = Constants.NotesSuccess;
        }
        private void NotesUpdateSuccess()
        {
            TempData["NoteUpdated"] = Constants.NoteUpdate;
        }

        private readonly ILogger<HomeController> _logger;
        
        UserServices _userService ;
        NotesServices _noteService ;
        public HomeController(ILogger<HomeController> logger,UserServices userServices,NotesServices notesServices)
        {
            _userService = userServices;
            _noteService = notesServices;
            _logger = logger;

        }
        
        [HttpGet]
        public IActionResult Index()
        {
            var UserName = SessionGetUserName();
            if (UserName == null)
                return View();
            else
            {
                var userData = _userService.GetUserWithNotes(UserName);

                if (userData == null)
                {
                    return RedirectToAction(Constants.LoginAction, "Account");
                }

                var model = _noteService.CreateHomeViewModel(userData);
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(Constants.CreateRoute);
        }
        [HttpPost]
        public IActionResult CreateNote(TaskEditViewModel model)
        {
            var UserName = SessionGetUserName();
            var userData = _userService.GetUser(UserName);

            if (userData == null)
            {
                return RedirectToAction("Login", "Account");
            }
            _noteService.CreateNotes(userData, model);

            NotesCreateSuccess();
            return RedirectToAction("Index");
        }
        [HttpPost]
        //Edit Notes 
        [HttpGet]
        public IActionResult GetNote(int id)
        {
            var UserName = SessionGetUserName();
            var userData = _userService.GetUserWithNotes(UserName);

            var noteData = _noteService.GetNote(userData, id);
            if (noteData == null)
            {
                return RedirectToAction(Constants.IndexAction);
            }

            var noteViewModel = _noteService.CreateViewModel(noteData);
            ViewBag.NoteId = id;
            return View(Constants.GetNoteRoute, noteViewModel);
        }
        [HttpPost]
        public IActionResult Edit(int id, TaskEditViewModel model)
        {
            var UserName = SessionGetUserName();
            var userData = _userService.GetUserWithNotes(UserName);
            var noteData = _noteService.GetNote(userData, id);

            if (noteData == null)
                return RedirectToAction(Constants.IndexAction);

            _noteService.CreateNoteVersion(noteData);
            _noteService.UpdateNote(noteData, model);
            NotesUpdateSuccess();
            return RedirectToAction(Constants.IndexAction);
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
                return RedirectToAction(Constants.IndexAction);

            _noteService.DeleteNote(note);
            return RedirectToAction(Constants.IndexAction);
        }
    }
}
