using FluentValidation;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;

namespace OrderProcessingSystem.Application.Validators.Orders
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator() {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one order item is required");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ItemId)
                    .NotEmpty();

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0);
            });
        }
    }
}
