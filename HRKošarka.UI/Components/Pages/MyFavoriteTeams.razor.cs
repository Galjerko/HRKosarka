using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages
{
    public partial class MyFavoriteTeams : PermissionBaseComponent
    {
        [Microsoft.AspNetCore.Components.Inject]
        private ITeamService TeamService { get; set; } = default!;

        private List<FavoriteTeamDTO> _favorites = new();
        private bool _isLoading = true;
        private bool _isProcessing = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFavorites();
        }

        private async Task LoadFavorites()
        {
            _isLoading = true;
            try
            {
                var response = await TeamService.GetMyFavoriteTeams();
                _favorites = response.IsSuccess ? response.Data ?? new() : new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading favorites: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task Unfollow(FavoriteTeamDTO team)
        {
            _isProcessing = true;
            try
            {
                var response = await TeamService.ToggleFavoriteTeam(team.TeamId);
                if (response.IsSuccess && !response.Data)
                {
                    _favorites.Remove(team);
                    Snackbar.Add($"Unfollowed {team.TeamName}.", Severity.Success);
                }
                else
                {
                    Snackbar.Add("Failed to unfollow team.", Severity.Error);
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
