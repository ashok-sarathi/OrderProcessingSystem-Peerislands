using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrderProcessingSystem.Api.Controllers;
using OrderProcessingSystem.Application.Dtos.Customers;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using System.Reflection;

namespace OrderProcessingSystem.Api.Tests.Controllers
{
    public class CustomersControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOk_WithCustomerList()
        {
            var expected = new List<CustomerDto>
            {
                new CustomerDto(Guid.NewGuid(), "Name1", "a@b.com", "111", "perm", "ship"),
                new CustomerDto(Guid.NewGuid(), "Name2", "c@d.com", "222", "perm2", "ship2")
            };

            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.Send(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var controller = new CustomersController(mockSender.Object);

            var actionResult = await controller.Get();

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Same(expected, Assert.IsType<List<CustomerDto>>(ok.Value));
        }

        [Fact]
        public async Task Post_InvalidModel_ReturnsBadRequestWithErrors()
        {
            var failure = new ValidationFailure("Name", "Name is required");
            var invalidResult = new ValidationResult(new[] { failure });

            var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
            mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidResult);

            var mockSender = new Mock<ISender>();
            var controller = new CustomersController(mockSender.Object);

            var request = new CreateCustomerRequest("", "e@x.com", "111", "p", "s");

            var actionResult = await controller.Post(request, mockValidator.Object);

            var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal(invalidResult.Errors, bad.Value);
        }

        [Fact]
        public async Task Post_ValidModel_ReturnsOkWithId()
        {
            var newId = Guid.NewGuid();

            var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
            mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); // valid

            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.Send(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);

            var controller = new CustomersController(mockSender.Object);

            var request = new CreateCustomerRequest("Jane", "jane@x.com", "555", "perm", "ship");

            var actionResult = await controller.Post(request, mockValidator.Object);

            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var valueObj = ok.Value!;
            var idProperty = valueObj.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            Assert.NotNull(idProperty);
            var returnedId = (Guid)idProperty!.GetValue(valueObj)!;
            Assert.Equal(newId, returnedId);
        }
    }
}
