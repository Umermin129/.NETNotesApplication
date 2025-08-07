using DataModel.Models;
using FastEndpoints;
using Services.Services;
using DataModel.Constants;
namespace FastEndPointUseNotes.Endpoints
{
    public class GetNoteEndpointClass : Ep.NoReq.NoRes
    {
        NotesServices _noteService;
        public GetNoteEndpointClass(NotesServices notesServices) {
            _noteService = notesServices;
        }
        public override void Configure()
        {
            Get("api/Home/GetNote/{NoteId}");
            AllowAnonymous();
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                var noteData = _noteService.GetNote(Route<int>("NoteId"));
                if (noteData == null)
                {
                    AddError(Constant.NoteNotFoundErr);
                    await Send.ErrorsAsync();
                }
                var noteViewModel = _noteService.CreateViewModel(noteData);

                await Send.OkAsync(new
                {
                    message = Constant.NoteRetrieveSuccess,
                    data = noteViewModel
                });
            }
            catch (Exception ex)
            {
                AddError(Constant.GetNoteErr);
                await Send.ErrorsAsync();
            }
        }
            
    }
}
