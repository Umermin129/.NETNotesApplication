using FastEndpoints;
using FluentValidation;
using System.Data;
using ViewModel.Registration;
namespace FastEndPointUseNotes.Validator
{
    public class RegistrationValidator : Validator<RegistrationViewModel>
    {
        public RegistrationValidator()
        {

            RuleFor(x => x.Sequence)
                .NotNull()
                .WithMessage("Please enter sequence")
                .When(x => x.Sequence == null);

            RuleFor(x => x.Sequence)
                .Must(seq => seq.Count == 5)
                .WithMessage("Please enter exactly 5 sequence values")
                .When(x => x.Sequence != null);
        }

    }
}
