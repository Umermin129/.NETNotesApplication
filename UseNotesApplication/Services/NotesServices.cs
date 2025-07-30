using Microsoft.EntityFrameworkCore;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels.Home;
using UseNotesApplication.ViewModels.Notes;

namespace UseNotesApplication.Services
{
    public class NotesServices
    {
        private readonly AppDbContext _context;
        //Create Notes
        public NotesServices(AppDbContext context)
        {
            _context = context;
        }
        public void CreateNotes(Users user, TaskEditViewModel model)
        {
            var note = new Notes
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status ?? "Pending",
                UsersId = user.Id,
            };
            _context.Notes.Add(note);
            _context.SaveChanges();
        }
        //Update Notes
        public void UpdateNote(Notes note, TaskEditViewModel model)
        {
            note.Title = model.Title;
            note.Description = model.Description;
            note.Status = model.Status ?? "Pending";
            note.LastModifiedAt = DateTime.UtcNow;
            _context.SaveChanges();
        }
        //Delete Note
        public void DeleteNote(Notes note)
        {
            note.IsDeleted = true;
            _context.SaveChanges();
        }
        //Get Notes
        public Notes GetNote(Users user,int id)
        {
            return user?.Notes.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
        }
        public Notes GetNote(int Id)
        {
            return _context.Notes.FirstOrDefault(u => u.Id == Id);
        }
        //DB to VM
        public TaskEditViewModel CreateViewModel(Notes note)
        {
            return new TaskEditViewModel
            {
                Title = note.Title,
                Description = note.Description,
                Status = note.Status,
                LastUpdated = note.LastModifiedAt,
            };
        }
        public HomeViewModel CreateHomeViewModel(Users user)
        {
            return new HomeViewModel
            {
                UserName = user.UserName,
                Name = user.Name,
                email = user.Email,
                TaskLists = user.Notes.Where(u => u.IsDeleted == false).Select(n => new TaskEditViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Description = n.Description,
                    Status = n.Status,
                    LastUpdated = n.LastModifiedAt,
                }).ToList()
            };
        }
        //Create DB
        public void CreateNoteVersion(Notes note)
        {
            var version = new NoteVersion
            {
                NotesId = note.Id,
                Title = note.Title,
                Description = note.Description,
                Status = note.Status,
                Timestamp = DateTime.UtcNow
            };
            _context.NoteVersions.Add(version);
        }
    }


}
