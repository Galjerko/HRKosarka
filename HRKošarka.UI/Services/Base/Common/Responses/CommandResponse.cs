namespace HRKošarka.UI.Services.Base.Common.Responses
{
    public class CommandResponse<T> : BaseResponse
    {
        public T? Data { get; set; }

        public static CommandResponse<T> Success(T data, string message = "Operation completed successfully")
        {
            return new CommandResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message
            };
        }

        public static CommandResponse<T> Failure(string message, List<string>? errors = null)
        {
            return new CommandResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}

