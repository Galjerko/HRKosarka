using HRKošarka.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HRKošarka.UI.Components.Shared
{
    public partial class NavMenu : ComponentBase
    {
        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

        private bool _isTeamRep = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true) return;
            if (user.IsInRole("Administrator") || user.IsInRole("ClubManager")) return;

            try
            {
                var response = await TeamService.GetMyRepresentativeships();
                _isTeamRep = response.IsSuccess && response.Data?.Any() == true;
            }
            catch { }
        }
    }
}
