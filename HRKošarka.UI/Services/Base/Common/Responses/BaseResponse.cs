namespace HRKošarka.UI.Services.Base.Common.Responses
{
    public abstract class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
