using Microsoft.EntityFrameworkCore;
using UseNotesApplication.Data;
using UseNotesApplication.Models;
using UseNotesApplication.ViewModels.Home;
using UseNotesApplication.ViewModels.Notes;
using UseNotesApplication.Controllers;
using System.Reflection.Metadata;
namespace UseNotesApplication.Services
{
    public class NotesServices
    {
        private readonly AppDbContext _context;
        //CreateNotes VM to DB
        public Notes NotesVMToDb(Users user,TaskEditViewModel model)
        {
            return new Notes
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status ?? "Pending",
                UsersId = user.Id,
            };
        }
        //Create Notes
        public NotesServices(AppDbContext context)
        {
            _context = context;
        }
        public void CreateNotes(Users user, TaskEditViewModel model)
        {
            var note = NotesVMToDb(user,model);
            try
            {
                _context.Notes.Add(note);
                _context.SaveChanges();
            }
            catch (Exception e)
            {

                throw new Exception($"{Constants.SaveNotesDB}: {e.Message}");
            }
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
            try
            {
                return user?.Notes.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constants.GetNoteErr}: {e.Message}");
            }
        }
        public Notes GetNote(int Id)
        {
            try
            {
                return _context.Notes.FirstOrDefault(u => u.Id == Id);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constants.GetNoteErr}: {e.Message}");
            }
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
            try
            {
                _context.NoteVersions.Add(version);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constants.SaveNoteVersionErr}: {e.Message}");
            }
        }
    }


}
