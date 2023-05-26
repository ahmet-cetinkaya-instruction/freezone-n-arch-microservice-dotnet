using FluentValidation;

namespace Application.Features.BasketItems.Commands.Checkout;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator() { }
}