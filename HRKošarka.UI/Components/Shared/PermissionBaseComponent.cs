using HRKošarka.UI.Contracts;
using HRKošarka.UI.Models.UserManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace HRKošarka.UI.Components.Base
{
    public class PermissionBaseComponent : ComponentBase, IAsyncDisposable
    {
        [Inject] protected IPermissionService PermissionService { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
        [Inject] protected ISnackbar Snackbar { get; set; } = default!;
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

        protected UserPermissions CurrentPermissions { get; set; } = new();
        protected ClaimsPrincipal? CurrentUser { get; set; }
        protected Guid? UserManagedClubId { get; set; }

        protected bool CanCreate { get; set; } = false;
        protected bool CanEdit { get; set; } = false;
        protected bool CanDeactivate { get; set; } = false;
        protected bool CanDelete { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadUserContext();
        }

        protected async Task LoadUserContext()
        {
            try
            {
                var authState = await AuthStateProvider.GetAuthenticationStateAsync();
                CurrentUser = authState.User;
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading user context: {ex.Message}", Severity.Error);
            }
        }

        protected async Task SetListPermissions()
        {
            try
            {
                CanCreate = CurrentUser.IsInRole("Administrator");
                CanEdit = CurrentUser.IsInRole("Administrator") || CurrentUser.IsInRole("ClubManager");
                CanDeactivate = CanEdit;
                CanDelete = CurrentUser.IsInRole("Administrator");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error setting permissions: {ex.Message}", Severity.Error);
            }
        }

        protected async Task SetClubPermissions(Guid clubId)
        {
            try
            {
                if (CurrentUser == null)
                {
                    await LoadUserContext();
                }

                CurrentPermissions = await PermissionService.GetPermissionsAsync(CurrentUser!, clubId);
                UserManagedClubId = CurrentPermissions.ManagedClubId;

            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error setting permissions: {ex.Message}", Severity.Error);
                NavigationManager.NavigateTo("/clubs");
            }
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await ValueTask.CompletedTask;
        }
    }
}
