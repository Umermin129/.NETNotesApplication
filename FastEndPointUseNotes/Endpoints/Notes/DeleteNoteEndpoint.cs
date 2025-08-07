using FastEndpoints;
using Services.Services;
using DataModel.Constants;
namespace FastEndPointUseNotes.Endpoints
{
    public class DeleteNoteEndpoint : Ep.NoReq.NoRes
    {
        NotesServices _noteService;
        public DeleteNoteEndpoint(NotesServices notesServices)
        {
            _noteService = notesServices;
        }
        public override void Configure()
        {
            Delete("/api/Home/DeleteNote/{NoteId}");
            AllowAnonymous();
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var note = _noteService.GetNote(Route<int>("NoteId"));

            if (note == null)
            {
                AddError(Constant.NoteNotFoundErr);
                await Send.ErrorsAsync();
            }

            _noteService.DeleteNote(note);
            await Send.OkAsync(new { message = "Note Deleted successfully." });
        }
    }
}

