using Blackwater.Core.Common.Exceptions.Models;

namespace Blackwater.Core.Common.Exceptions
{
    public class ValidationException(IEnumerable<string> errors) : CoreException("Validation failed.")
    {
        public IEnumerable<string> ValidationErrors { get; } = errors;
    }

    public class ServiceUnavailableException(string serviceName) : CoreException($"{serviceName} is currently unavailable.", "SERVICE_UNAVAILABLE")
    {
    }

    public class OperationFailedException(string operationName, Exception innerException) : CoreException($"The operation '{operationName}' has failed.", "OPERATION_FAILED", innerException)
    {
        public string OperationName { get; } = operationName;
    }
}
