using FluentValidation;
using HafezLibrary.Models;

namespace HafezLibrary.Validators
{
    public class NotificationValidator : AbstractValidator<NotificationModel>
    {
        public NotificationValidator()
        {
            RuleFor(n => n.Description)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage("توضیحات اعلان نباید خالی باشد");
            //.Length(2, 50).WithMessage("لطفا مقدار مناسبی برای توضیحات وارد کنید");
        }
    }
}