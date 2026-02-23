using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class AssignClubManager : PermissionBaseComponent
    {
        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public NonAdminUserDTO User { get; set; } = default!;

        [Inject] public IClubService ClubService { get; set; } = default!;
        [Inject] public IUserService UserService { get; set; } = default!;

        protected List<ClubWithoutManagerDTO> _clubs = new();
        protected Guid? _selectedClubId = null;
        protected string _searchTerm = string.Empty;
        protected bool _loading;
        protected bool _saving;

        private MudSelect<Guid?>? _clubSelect;
        private bool _openAfterSearch;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadClubsAsync();
        }

        protected async Task OnSearchChanged()
        {
            _openAfterSearch = true;
            await LoadClubsAsync();
        }

        private async Task LoadClubsAsync()
        {
            _loading = true;
            StateHasChanged();

            try
            {
                var response = await ClubService.GetClubsWithoutManagerAsync(_searchTerm);

                if (response.IsSuccess && response.Data != null)
                {
                    _clubs = response.Data.ToList();

                    if (_selectedClubId is not null &&
                        !_clubs.Any(c => c.Id == _selectedClubId.Value))
                        _selectedClubId = null;
                }
                else
                {
                    _clubs.Clear();
                    _selectedClubId = null;
                    Snackbar.Add(response.Message ?? "Failed to load clubs without manager.", Severity.Error);
                }
            }
            finally
            {
                _loading = false;
                StateHasChanged();

                if (_openAfterSearch && _clubs.Any() && _clubSelect is not null)
                {
                    await _clubSelect.OpenMenu();
                }

                _openAfterSearch = false;
            }
        }

        protected void Cancel() => MudDialog.Cancel();

        protected async Task Save()
        {
            if (_selectedClubId is null)
            {
                Snackbar.Add("Please select a club.", Severity.Warning);
                return;
            }

            _saving = true;
            StateHasChanged();

            try
            {
                var cmd = new AssignClubManagerCommand
                {
                    UserId = User.Id,
                    ClubId = _selectedClubId.Value
                };

                var result = await UserService.AssignClubManager(cmd);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message ?? "Failed to assign club manager.", Severity.Error);
                    return;
                }

                MudDialog.Close(DialogResult.Ok(true));
            }
            finally
            {
                _saving = false;
                StateHasChanged();
            }
        }
    }
}
