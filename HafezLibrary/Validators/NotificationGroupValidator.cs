using FluentValidation;
using HafezLibrary.Models;

namespace HafezLibrary.Validators
{
    public class NotificationGroupValidator : AbstractValidator<NotificationGroupModel>
    {
        public NotificationGroupValidator()
        {
            RuleFor(n => n.Name)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("نام گروه نباید خالی باشد")
                .Length(2, 250).WithMessage("لطفا مقدار مناسبی برای نام گروه وارد کنید");
        }
    }
}