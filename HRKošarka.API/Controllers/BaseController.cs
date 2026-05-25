using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRKošarka.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected bool IsAdmin => User.IsInRole("Administrator");

        // Null when caller is admin — handlers interpret null as "bypass auth check"
        protected string? CallerClubId => IsAdmin ? null : User.FindFirstValue("ClubId");
        protected string? CallerUserId => IsAdmin ? null : User.FindFirstValue("uid");

        // Always the caller's own ID — for audit fields (ConfirmedByUserId etc.)
        protected string CurrentUserId => User.FindFirstValue("uid") ?? string.Empty;

        // For the few commands that store club as Guid? instead of string?
        protected Guid? CallerClubGuid =>
            Guid.TryParse(CallerClubId, out var id) ? id : (Guid?)null;
    }
}
