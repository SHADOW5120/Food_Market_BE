using System.Net;

namespace Food_Market_BE.Shared.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public object Errors { get; }

        public AppException(string message, int statusCode = (int)HttpStatusCode.BadRequest, object errors = null)
            : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}