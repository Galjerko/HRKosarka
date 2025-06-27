namespace HRKošarka.Application.Models.Requests
{
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
        public string? SearchTerm { get; set; }
        public List<string> SearchableProperties { get; set; } = new();
        public List<string> SortableProperties { get; set; } = new();
    }
}
