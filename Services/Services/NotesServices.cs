using Microsoft.EntityFrameworkCore;
using DataModel.Data;
using DataModel.Models;
using DataModel.Constants;
using Mapster;
using ViewModel.Home;
using ViewModel.Notes;
namespace Services.Services
{
    public class NotesServices
    {
        private readonly AppDbContext _context;
        //CreateNotes VM to DB
        //Create Notes
        public NotesServices(AppDbContext context)
        {
            _context = context;
        }
        public void CreateNotes(Users user, TaskEditViewModel model)
        {

            var note = model.Adapt<Notes>();
            note.UsersId = user.Id;
            try
            {
                _context.Notes.Add(note);
                _context.SaveChanges();
                //Commit Transaction
            }
            catch (Exception e)
            {

                throw new Exception($"{Constant.SaveNotesDB}: {e.Message}");
            }


        }

        //Update Notes
        public void UpdateNote(Notes note, TaskEditViewModel model)
        {
                try
                {
                    model.Adapt(note);
                    note.LastModifiedAt = DateTime.UtcNow;
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception($"{Constant.UpdateNotesDb}: {e.Message}");
                }
        }
        //Delete Note
        public void DeleteNote(Notes note)
        {
            note.IsDeleted = true;
            _context.SaveChanges();
        }
        //Get Notes

        public Notes GetNote(int Id)
        {
            try
            {
                return _context.Notes.FirstOrDefault(u => u.Id == Id);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.GetNoteErr}: {e.Message}");
            }
        }
        //DB to VM
        public TaskEditViewModel CreateViewModel(Notes note)
        {

            var config = new TypeAdapterConfig();
            config.NewConfig<Notes, TaskEditViewModel>()
                  .Map(dest => dest.LastUpdated, src => src.LastModifiedAt);

            var taskModel = note.Adapt<TaskEditViewModel>(config);
            return taskModel;

        }
        public HomeViewModel CreateHomeViewModel(Users user)
        {
            var homeModel = user.Adapt<HomeViewModel>();
            homeModel.TaskLists = user.Notes
                .Where(n => !n.IsDeleted)
                .Select(n => CreateViewModel(n))
                .ToList();
            return homeModel;
        }
        //Create DB
        public void CreateNoteVersion(Notes note)
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<Notes, NoteVersion>()
                .Ignore(dest => dest.Id) // Let EF auto-generate the ID for NoteVersion
                .Map(dest => dest.NotesId, src => src.Id) // Use Notes.Id as foreign key
                .Map(dest => dest.Timestamp, src => DateTime.UtcNow); // Optional: force-set timestamp

            var noteVersion = note.Adapt<NoteVersion>(config);
            try
            {
                _context.NoteVersions.Add(noteVersion);
            }
            catch (Exception e)
            {
                throw new Exception($"{Constant.SaveNoteVersionErr}: {e.Message}");
            }
        }
    }


}
