namespace HRKošarka.Application.Models.Identity
{
    public class ClubManagerResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public static ClubManagerResult Success() => new ClubManagerResult { IsSuccess = true };
        public static ClubManagerResult Failure(string message) => new ClubManagerResult { IsSuccess = false, Message = message };
    }
}
