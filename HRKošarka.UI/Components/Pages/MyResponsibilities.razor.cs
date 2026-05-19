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

        private List<PendingActionDTO> _actions = new();
        private ClubDetailsDTO? _myClub;
        private bool _isLoading = true;
        private bool _isClubManager = false;
        private readonly HashSet<int> _collapsedGroups = new();
        private bool _teamsExpanded = true;

        private void ToggleGroup(int key)
        {
            if (!_collapsedGroups.Add(key))
                _collapsedGroups.Remove(key);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isClubManager = CurrentUser?.IsInRole("ClubManager") == true;

            var tasks = new List<Task> { LoadActions() };
            if (_isClubManager)
                tasks.Add(LoadMyClub());

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

        private string? GetClubLogo() =>
            _myClub?.ImageBytes?.Length > 0 && !string.IsNullOrEmpty(_myClub.ImageContentType)
                ? $"data:{_myClub.ImageContentType};base64,{Convert.ToBase64String(_myClub.ImageBytes)}"
                : null;
    }
}
