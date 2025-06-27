namespace HRKošarka.Application.Models.Responses
{
    public class QueryResponse<T> : BaseResponse
    {
        public T? Data { get; set; }

        public static QueryResponse<T> Success(T data, string message = "Data retrieved successfully")
        {
            return new QueryResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message
            };
        }

        public static QueryResponse<T> Failure(string message)
        {
            return new QueryResponse<T>
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
