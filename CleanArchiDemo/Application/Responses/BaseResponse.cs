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

        //Ok = 200, BadRequest = 400, NotFound = 404
        public enum StatusCode { Ok, BadRequest, NotFound };

        public StatusCode Success { get; set; }
        public string Message { get; set; }
        public List<string> ValidationErrors { get; set; }
    }
}
