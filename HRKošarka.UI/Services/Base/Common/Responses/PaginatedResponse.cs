namespace HRKošarka.UI.Services.Base.Common.Responses
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

        //public class PaginationMetadata
        //{
        //    public int CurrentPage { get; set; }
        //    public int TotalPages { get; set; }
        //    public int PageSize { get; set; }
        //    public int TotalCount { get; set; }
        //    public bool HasPrevious => CurrentPage > 1;
        //    public bool HasNext => CurrentPage < TotalPages;
        //}
    }
}
