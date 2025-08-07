using FastEndpoints;
using ViewModel.Registration;
using Services.Services;
using FastEndPointUseNotes.Validator;
namespace FastEndPointUseNotes.Endpoints.Register
{
    public class RegisterEndpoint : Endpoint<RegistrationViewModel>
    {
        private readonly UserServices _services;
        public RegisterEndpoint(UserServices services)
        {
            _services = services;
        }
        public override void Configure()
        {
            Post("api/Account/Register");
            AllowAnonymous();
            AllowFileUploads();
            AllowFormData();
            Validator<RegistrationValidator>();
        }
        public override async Task HandleAsync(RegistrationViewModel model, CancellationToken ct)
        {
            try
            {
                var response = _services.VmToDb(model);
                if(response.StatusCode!=200)
                {
                    AddError($"Validation Error : {response.Message}");
                    await Send.ErrorsAsync(response.StatusCode);
                }
                else
                {
                    await Send.OkAsync(response);
                }
            }
            catch (Exception ex)
            {
                AddError($"Error: {ex.Message}");
                await Send.ErrorsAsync();
            }
        }
    }
}
