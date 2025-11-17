// Models/ApiResponse.cs
namespace FormBuilder.API.Models
{
    public class SingleApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int StatusCode { get; set; }

        public static SingleApiResponse SuccessResult(object data = null, string message = "Operation completed successfully")
        {
            return new SingleApiResponse
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static SingleApiResponse ErrorResult(string message, List<string> errors = null, int statusCode = 400)
        {
            return new SingleApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                StatusCode = statusCode
            };
        }

        public static SingleApiResponse NotFoundResult(string message = "Resource not found")
        {
            return new SingleApiResponse
            {
                Success = false,
                Message = message,
                StatusCode = 404
            };
        }

        public static SingleApiResponse UnauthorizedResult(string message = "Unauthorized access")
        {
            return new SingleApiResponse
            {
                Success = false,
                Message = message,
                StatusCode = 401
            };
        }
    }
}