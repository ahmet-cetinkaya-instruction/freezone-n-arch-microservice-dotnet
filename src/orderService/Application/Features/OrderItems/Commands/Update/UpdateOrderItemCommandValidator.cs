using FluentValidation;

namespace Application.Features.OrderItems.Commands.Update;

public class UpdateOrderItemCommandValidator : AbstractValidator<UpdateOrderItemCommand>
{
    public UpdateOrderItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
        RuleFor(c => c.ProductName).NotEmpty();
        RuleFor(c => c.UnitPrice).NotEmpty();
        RuleFor(c => c.Quantity).NotEmpty();
        RuleFor(c => c.Address).NotEmpty();
    }
}