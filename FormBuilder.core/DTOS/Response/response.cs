namespace FormBuilder.API.Models
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string ?Message { get; set; }
        public object ?Data { get; set; }
        public DateTime ?Timestamp { get; set; }

        // Constructor بمعاملين
        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Timestamp = DateTime.UtcNow;
        }

        // Constructor بثلاثة معاملات
        public ApiResponse(int statusCode, string message, object data)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
            Data = data;
            Timestamp = DateTime.UtcNow;
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                200 => "Success",
                201 => "Created",
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Resource Not Found",
                500 => "Internal Server Error",
                _ => "Unknown Status"
            };
        }
    }
}