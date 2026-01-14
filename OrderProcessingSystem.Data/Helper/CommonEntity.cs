using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Data.Helper
{
    [ExcludeFromCodeCoverage]
    public abstract class CommonEntity
    {
        public Guid Id { get; set; }
    }
}
