using FluentValidation;

namespace Application.Features.OrderItems.Commands.Create;

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
        RuleFor(c => c.ProductName).NotEmpty();
        RuleFor(c => c.UnitPrice).NotEmpty();
        RuleFor(c => c.Quantity).NotEmpty();
        RuleFor(c => c.Address).NotEmpty();
    }
}