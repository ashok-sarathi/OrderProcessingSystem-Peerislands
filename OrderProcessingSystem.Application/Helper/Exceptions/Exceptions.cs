using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Helper.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class BadRequestException(string message) : Exception(message)
    {
    }

    [ExcludeFromCodeCoverage]
    public class NotFoundException(string message) : Exception(message)
    {
    }
}
