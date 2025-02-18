namespace Blackwater.Core.Common.Exceptions.Models
{
    public class CoreException : Exception
    {
        public string ErrorCode { get; } = String.Empty;
        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public CoreException() { }

        public CoreException(string message) : base(message) { }

        public CoreException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public CoreException(string message, Exception innerException) : base(message, innerException) { }

        public CoreException(string message, string errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
