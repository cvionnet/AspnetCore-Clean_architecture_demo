using System.Collections.Generic;

namespace Application.Responses
{
    /// <summary>
    /// Standard response send by the controller
    /// </summary>
    public class BaseResponse
    {
        public BaseResponse()
        {
            Success = StatusCode.Ok;
        }

        public BaseResponse(string message = null)
        {
            Success = StatusCode.Ok;
            Message = message;
        }

        public BaseResponse(string message, StatusCode success)
        {
            Success = success;
            Message = message;
        }

        // NotFound = 404, Ok = 200, BadRequest = 400
        public enum StatusCode { NotFound, Ok, BadRequest };

        public StatusCode Success { get; set; }
        public string Message { get; set; }
        public List<string> ValidationErrors { get; set; }
    }
}
