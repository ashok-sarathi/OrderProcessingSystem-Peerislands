using FluentValidation;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;

namespace OrderProcessingSystem.Application.Validators.Customers
{
    public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
    {
        public CreateCustomerRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(50).WithMessage("Email cannot exceed 50 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters.");

            RuleFor(x => x.PermanentAddress)
                .NotEmpty().WithMessage("Permanent Address is required.")
                .MaximumLength(50).WithMessage("Permanent Address cannot exceed 50 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Shipping Address is required.")
                .MaximumLength(50).WithMessage("Shipping Address cannot exceed 50 characters.");
        }
    }
}
