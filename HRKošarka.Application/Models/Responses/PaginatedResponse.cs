namespace HRKošarka.Application.Models.Responses
{
    public class PaginatedResponse<T> : BaseResponse
    {
        public IList<T> Data { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; } = new();

        public static PaginatedResponse<T> Success(
            IList<T> data,
            int currentPage,
            int pageSize,
            int totalCount,
            string message = "Data retrieved successfully")
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new PaginatedResponse<T>
            {
                Data = data,
                IsSuccess = true,
                Message = message,
                Pagination = new PaginationMetadata
                {
                    CurrentPage = currentPage,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages
                }
            };
        }

        public static PaginatedResponse<T> Failure(string message, List<string>? errors)
        {
            return new PaginatedResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
