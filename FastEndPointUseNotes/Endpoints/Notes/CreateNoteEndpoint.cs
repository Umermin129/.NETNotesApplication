using FastEndpoints;
using Services.Services;
using ViewModel.Notes;
namespace FastEndPointUseNotes.Endpoints
{
    public class CreateNoteEndpoint : Endpoint<TaskEditViewModel>
    {
        UserServices _userService;
        NotesServices _noteService;
        public CreateNoteEndpoint(UserServices userServices, NotesServices notesServices)
        {
            _userService = userServices;
            _noteService = notesServices;
        }
        public override void Configure()
        {
            Post("api/Home/CreateNote/{UserName}");
            AllowAnonymous();
        }
        public override async Task HandleAsync(TaskEditViewModel model,CancellationToken ct)
        {
            try
            {
                var userData = _userService.GetUser(Route<string>("UserName"));
                var response = _noteService.CreateNotes(userData, model);
                await Send.OkAsync(response);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                await Send.ErrorsAsync();
            }
        }
    }
}
