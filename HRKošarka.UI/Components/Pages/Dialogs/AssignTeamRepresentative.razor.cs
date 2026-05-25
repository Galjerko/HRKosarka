using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class AssignTeamRepresentative : PermissionBaseComponent
    {
        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Guid TeamId { get; set; }

        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private IUserService UserService { get; set; } = default!;

        private List<NonAdminUserDTO> _users = new();
        private NonAdminUserDTO? _selectedUser;
        private string _searchTerm = string.Empty;
        private bool _loading;
        private bool _saving;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadUsers();
        }

        private async Task SearchUsers() => await LoadUsers();

        private async Task LoadUsers()
        {
            _loading = true;
            StateHasChanged();
            try
            {
                var response = await UserService.GetUsers(new PaginationRequest
                {
                    Page = 1,
                    PageSize = 50,
                    SearchTerm = string.IsNullOrWhiteSpace(_searchTerm) ? null : _searchTerm
                });
                _users = response.IsSuccess ? response.Data?.ToList() ?? new() : new();
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private void SelectUser(NonAdminUserDTO user)
        {
            _selectedUser = _selectedUser?.Id == user.Id ? null : user;
        }

        private void Cancel() => MudDialog.Cancel();

        private async Task Save()
        {
            if (_selectedUser == null) return;

            _saving = true;
            StateHasChanged();
            try
            {
                var response = await TeamService.AssignTeamRepresentative(TeamId, new AssignTeamRepresentativeCommand
                {
                    TeamId = TeamId,
                    UserId = _selectedUser.Id
                });

                if (response.IsSuccess)
                    MudDialog.Close(DialogResult.Ok(true));
                else
                    Snackbar.Add(response.Message ?? "Failed to assign representative.", Severity.Error);
            }
            finally
            {
                _saving = false;
                StateHasChanged();
            }
        }
    }
}
