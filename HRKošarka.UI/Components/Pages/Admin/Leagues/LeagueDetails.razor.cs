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
        private List<LeagueBreakDTO> _breaks = new();
        private List<LeagueRoundDTO> _schedule = new();
        private LeagueStandingsDTO? _standings;

        private bool _isLoading = true;
        private bool _isLoadingTeams = false;
        private bool _isLoadingBreaks = false;
        private bool _isLoadingSchedule = false;
        private bool _isLoadingStandings = false;
        private bool _isProcessing = false;

        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;
        private bool _showRemoveTeamDialog = false;
        private bool _showGenerateScheduleDialog = false;

        private LeagueTeamDTO? _teamToRemove;
        private LeagueBreakDTO? _breakToRemove;
        private bool _showRemoveBreakDialog = false;

        // Add break inline form
        private string _breakName = string.Empty;
        private DateTime? _breakStartDate;
        private DateTime? _breakEndDate;
        private bool _isAddingBreak = false;

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
            await Task.WhenAll(LoadLeagueDetails(), LoadLeagueTeams(), LoadBreaks());
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
                    if (_league.ScheduleGenerated)
                        await Task.WhenAll(LoadSchedule(), LoadStandings());
                }
                else
                {
                    foreach (var error in response.Errors?.Any() == true
                        ? response.Errors
                        : new List<string> { response.Message ?? "Failed to load league details." })
                        Snackbar.Add(error, Severity.Error);
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

        private async Task LoadBreaks()
        {
            _isLoadingBreaks = true;
            try
            {
                var response = await LeagueService.GetLeagueBreaks(Id);
                if (response.IsSuccess)
                    _breaks = response.Data ?? new List<LeagueBreakDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading breaks: {ex.Message}");
            }
            finally
            {
                _isLoadingBreaks = false;
            }
        }

        private async Task LoadSchedule()
        {
            _isLoadingSchedule = true;
            try
            {
                var response = await LeagueService.GetLeagueSchedule(Id);
                if (response.IsSuccess)
                    _schedule = response.Data ?? new List<LeagueRoundDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading schedule: {ex.Message}");
            }
            finally
            {
                _isLoadingSchedule = false;
            }
        }

        private async Task LoadStandings()
        {
            _isLoadingStandings = true;
            try
            {
                var response = await LeagueService.GetLeagueStandings(Id);
                if (response.IsSuccess)
                    _standings = response.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading standings: {ex.Message}");
            }
            finally
            {
                _isLoadingStandings = false;
            }
        }

        private async Task AddBreak()
        {
            if (string.IsNullOrWhiteSpace(_breakName) || !_breakStartDate.HasValue || !_breakEndDate.HasValue)
            {
                Snackbar.Add("Please fill in all break fields.", Severity.Warning);
                return;
            }

            _isAddingBreak = true;
            try
            {
                var command = new AddLeagueBreakCommand
                {
                    LeagueId = Id,
                    Name = _breakName.Trim(),
                    StartDate = _breakStartDate.Value,
                    EndDate = _breakEndDate.Value
                };

                var response = await LeagueService.AddLeagueBreak(Id, command);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Break added successfully.", Severity.Success);
                    _breakName = string.Empty;
                    _breakStartDate = null;
                    _breakEndDate = null;
                    await LoadBreaks();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to add break.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error adding break: {ex.Message}");
            }
            finally
            {
                _isAddingBreak = false;
            }
        }

        private void RemoveBreak(LeagueBreakDTO b)
        {
            _breakToRemove = b;
            _showRemoveBreakDialog = true;
        }

        private async Task ConfirmRemoveBreak()
        {
            if (_breakToRemove == null) return;
            _isProcessing = true;
            try
            {
                var response = await LeagueService.RemoveLeagueBreak(Id, _breakToRemove.Id);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Break removed.", Severity.Success);
                    _showRemoveBreakDialog = false;
                    await LoadBreaks();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to remove break.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error removing break: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                _breakToRemove = null;
            }
        }

        private async Task ConfirmGenerateSchedule()
        {
            _isProcessing = true;
            try
            {
                var response = await LeagueService.GenerateLeagueSchedule(Id);
                if (response.IsSuccess)
                {
                    Snackbar.Add(response.Message ?? "Schedule generated!", Severity.Success);
                    _showGenerateScheduleDialog = false;
                    await LoadLeagueDetails(); // internally calls LoadSchedule when ScheduleGenerated = true
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to generate schedule.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error generating schedule: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
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
            if (_league == null) return;
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
                    foreach (var e in response.Errors?.Any() == true ? response.Errors : new List<string> { response.Message ?? "Failed to deactivate league." })
                        Snackbar.Add(e, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally { _isProcessing = false; }
        }

        private async Task ConfirmActivate()
        {
            if (_league == null) return;
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
                    foreach (var e in response.Errors?.Any() == true ? response.Errors : new List<string> { response.Message ?? "Failed to activate league." })
                        Snackbar.Add(e, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally { _isProcessing = false; }
        }

        private async Task ConfirmDelete()
        {
            if (_league == null) return;
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
                    foreach (var e in response.Errors?.Any() == true ? response.Errors : new List<string> { response.Message ?? "Failed to delete league." })
                        Snackbar.Add(e, Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally { _isProcessing = false; }
        }

        private LeagueTeamDTO? GetByeTeam(LeagueRoundDTO round)
        {
            // Byes in cups only happen in round 1 (when team count is not a power of 2).
            // All later rounds have no byes — absent teams are eliminated, not waiting.
            if (_league?.CompetitionType == CompetitionType._1 && round.Round != 1)
                return null;

            var playingIds = new HashSet<Guid>();
            foreach (var m in round.Matches) { playingIds.Add(m.HomeTeamId); playingIds.Add(m.AwayTeamId); }
            return _leagueTeams.FirstOrDefault(t => !playingIds.Contains(t.TeamId));
        }

        private bool IsTournamentFinished =>
            _league?.ScheduleGenerated == true
            && _schedule.Any()
            && _schedule.All(r => r.Matches.All(m =>
                m.Status == MatchStatus._2 || m.Status == MatchStatus._3));
    }
}
