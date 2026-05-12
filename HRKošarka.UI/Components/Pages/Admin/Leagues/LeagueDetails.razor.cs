using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Components.Pages.Dialogs;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Leagues
{
    public partial class LeagueDetails : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private LeagueDetailsDTO? _league;
        private List<LeagueTeamDTO> _leagueTeams = new();
        private bool _isLoading = true;
        private bool _isLoadingTeams = false;
        private bool _isProcessing = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;
        private bool _showRemoveTeamDialog = false;
        private LeagueTeamDTO? _teamToRemove;

        private string _deactivateMessage => _league is null
            ? string.Empty
            : $"Are you sure you want to deactivate <strong>{_league.Name}</strong>?";

        private string _activateMessage => _league is null
            ? string.Empty
            : $"Are you sure you want to activate <strong>{_league.Name}</strong>?";

        private string _deleteMessage => _league is null
            ? string.Empty
            : $"Are you sure you want to permanently delete <strong>{_league.Name}</strong>?";

        private readonly DialogOptions _dialogOptions = new()
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Task.WhenAll(LoadLeagueDetails(), LoadLeagueTeams());
        }

        private async Task LoadLeagueDetails()
        {
            _isLoading = true;
            try
            {
                var response = await LeagueService.GetLeagueById(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _league = response.Data;
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                            Snackbar.Add(error, Severity.Error);
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to load league details.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading league details.", Severity.Error);
                Console.WriteLine($"Error loading league details: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadLeagueTeams()
        {
            _isLoadingTeams = true;
            try
            {
                var response = await LeagueService.GetLeagueTeams(Id);
                if (response.IsSuccess)
                    _leagueTeams = response.Data ?? new List<LeagueTeamDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading league teams: {ex.Message}");
            }
            finally
            {
                _isLoadingTeams = false;
            }
        }

        private async Task OpenRegisterTeamDialog()
        {
            var parameters = new DialogParameters
            {
                ["LeagueId"] = Id,
                ["LeagueStartDate"] = _league!.StartDate,
                ["LeagueEndDate"] = _league!.EndDate
            };
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
            var dialog = await DialogService.ShowAsync<RegisterTeamInLeague>("Register Team", parameters, options);
            var result = await dialog.Result;

            if (result is { Canceled: false })
            {
                Snackbar.Add("Team registered successfully!", Severity.Success);
                await LoadLeagueTeams();
            }
        }

        private void RemoveTeam(LeagueTeamDTO team)
        {
            _teamToRemove = team;
            _showRemoveTeamDialog = true;
        }

        private async Task ConfirmRemoveTeam()
        {
            if (_teamToRemove == null) return;

            _isProcessing = true;
            try
            {
                var response = await LeagueService.RemoveTeamFromLeague(Id, _teamToRemove.TeamId);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Team removed from league.", Severity.Success);
                    _showRemoveTeamDialog = false;
                    await LoadLeagueTeams();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to remove team.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                _teamToRemove = null;
            }
        }

        private async Task ConfirmDeactivate()
        {
            if (_league == null)
            {
                return;
            }

            _isProcessing = true;
            try
            {
                var response = await LeagueService.DeactivateLeague(_league.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("League deactivated successfully!", Severity.Success);
                    _showDeactivateDialog = false;
                    await LoadLeagueDetails();
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var e in response.Errors)
                        {
                            Snackbar.Add(e, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to deactivate league.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task ConfirmActivate()
        {
            if (_league == null)
            {
                return;
            }

            _isProcessing = true;
            try
            {
                var response = await LeagueService.ActivateLeague(_league.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("League activated successfully!", Severity.Success);
                    _showActivateDialog = false;
                    await LoadLeagueDetails();
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var e in response.Errors)
                        {
                            Snackbar.Add(e, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to activate league.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task ConfirmDelete()
        {
            if (_league == null)
            {
                return;
            }

            _isProcessing = true;
            try
            {
                var response = await LeagueService.DeleteLeague(_league.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("League deleted successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/leagues");
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var e in response.Errors)
                        {
                            Snackbar.Add(e, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to delete league.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
