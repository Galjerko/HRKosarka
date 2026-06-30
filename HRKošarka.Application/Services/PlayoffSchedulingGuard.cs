using HRKošarka.Application.Exceptions;

namespace HRKošarka.Application.Services
{
    public static class PlayoffSchedulingGuard
    {
        public static void EnsureWithinCapDate(DateTime date, DateTime capDate)
        {
            if (date > capDate)
                throw new BadRequestException(
                    $"Playoff scheduling would exceed the configured end date ({capDate:dd.MM.yyyy}). " +
                    "Adjust the playoff end date.");
        }
    }
}
