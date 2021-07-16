using FluentValidation;
using HafezLibrary.Models;

namespace HafezLibrary.Validators
{
    public class AddUserValidator : AbstractValidator<UserModel>
    {
        public AddUserValidator()
        {
            RuleFor(U => U.UserId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("شناسه کاربری نباید خالی باشد")
                .Length(0, 10).WithMessage("اندازه شناسه کاربری کافی نمیباشد");
        }
    }
}