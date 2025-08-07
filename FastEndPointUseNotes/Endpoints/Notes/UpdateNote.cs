using DataModel.Models;
using FastEndpoints;
using Services.Services;
using ViewModel.Notes;
using DataModel.Constants;

using Microsoft.AspNetCore.Http.HttpResults;
namespace FastEndPointUseNotes.Endpoints
{
    public class UpdateNote : Endpoint<TaskEditViewModel>
    {
        NotesServices _noteService;
        public UpdateNote(NotesServices notesServices)
        {
            _noteService = notesServices;
        }
        public override void Configure()
        {
            Put("/api/Home/UpdateNote/{NoteId}");
            AllowAnonymous();
        }
        public override async Task HandleAsync(TaskEditViewModel model,CancellationToken ct)
        {
            try
            {

                var noteData = _noteService.GetNote(Route<int>("NoteId"));
                if (noteData == null)
                {
                    AddError(Constant.NoteNotFoundErr);
                    await Send.ErrorsAsync();
                }

                _noteService.CreateNoteVersion(noteData);
                var response = _noteService.UpdateNote(noteData, model);

                await Send.OkAsync(new {message = response.Message,data = response.ModelObject});
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                await Send.ErrorsAsync();
            }
        }
    }
}
