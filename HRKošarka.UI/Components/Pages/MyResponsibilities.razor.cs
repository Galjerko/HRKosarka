using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace HRKošarka.UI.Components.Pages
{
    public partial class MyResponsibilities : PermissionBaseComponent
    {
        [Inject] private IMatchService MatchService { get; set; } = default!;
        [Inject] private IClubService ClubService { get; set; } = default!;
        [Inject] private ITeamService TeamService { get; set; } = default!;

        private List<PendingActionDTO> _actions = new();
        private ClubDetailsDTO? _myClub;
        private List<TeamRepMembershipDTO> _myRepTeams = new();
        private bool _isLoading = true;
        private bool _isClubManager = false;
        private bool _isTeamRep = false;
        private readonly HashSet<int> _collapsedGroups = new();
        private bool _teamsExpanded = true;
        private bool _repTeamsExpanded = true;

        private void ToggleGroup(int key)
        {
            if (!_collapsedGroups.Add(key))
                _collapsedGroups.Remove(key);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isClubManager = CurrentUser?.IsInRole("ClubManager") == true;
            bool isAdmin = CurrentUser?.IsInRole("Administrator") == true;

            var tasks = new List<Task> { LoadActions() };
            if (_isClubManager)
                tasks.Add(LoadMyClub());
            else if (!isAdmin)
                tasks.Add(LoadMyRepTeams());

            await Task.WhenAll(tasks);
            _isLoading = false;
        }

        private async Task LoadActions()
        {
            try
            {
                var response = await MatchService.GetPendingActions();
                if (response.IsSuccess)
                    _actions = response.Data ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error loading actions: {ex.Message}"); }
        }

        private async Task LoadMyClub()
        {
            try
            {
                var clubIdStr = CurrentUser?.FindFirst("ClubId")?.Value;
                if (Guid.TryParse(clubIdStr, out var clubId))
                {
                    var response = await ClubService.GetClubDetails(clubId);
                    if (response.IsSuccess)
                        _myClub = response.Data;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error loading club: {ex.Message}"); }
        }

        private async Task LoadMyRepTeams()
        {
            try
            {
                var response = await TeamService.GetMyRepresentativeships();
                if (response.IsSuccess)
                {
                    _myRepTeams = response.Data ?? new();
                    _isTeamRep = _myRepTeams.Any();
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error loading rep teams: {ex.Message}"); }
        }

        private string? GetClubLogo() =>
            _myClub?.ImageBytes?.Length > 0 && !string.IsNullOrEmpty(_myClub.ImageContentType)
                ? $"data:{_myClub.ImageContentType};base64,{Convert.ToBase64String(_myClub.ImageBytes)}"
                : null;
    }
}
