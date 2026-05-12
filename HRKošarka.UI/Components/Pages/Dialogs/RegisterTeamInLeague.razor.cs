using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class RegisterTeamInLeague
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Guid LeagueId { get; set; }
        [Parameter] public DateTime LeagueStartDate { get; set; }
        [Parameter] public DateTime LeagueEndDate { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private List<AvailableTeamForLeagueDTO> _teams = new();
        private AvailableTeamForLeagueDTO? _selectedTeam;
        private string _searchTerm = string.Empty;
        private DateTime? _registrationDate;
        private bool _loadingTeams = false;
        private bool _saving = false;

        private bool CanSave => _selectedTeam is not null && _registrationDate.HasValue && !_saving;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _registrationDate = DateTime.Today <= LeagueEndDate && DateTime.Today >= LeagueStartDate
                ? DateTime.Today
                : LeagueStartDate;
            await LoadTeams();
        }

        private async Task LoadTeams()
        {
            _loadingTeams = true;
            try
            {
                var response = await LeagueService.GetAvailableTeamsForLeague(LeagueId, _searchTerm);
                _teams = response.IsSuccess ? response.Data ?? new List<AvailableTeamForLeagueDTO>() : new List<AvailableTeamForLeagueDTO>();
            }
            finally
            {
                _loadingTeams = false;
                StateHasChanged();
            }
        }

        private async Task OnSearchChanged(string value)
        {
            _searchTerm = value;
            await LoadTeams();
        }

        private void SelectTeam(AvailableTeamForLeagueDTO team)
        {
            _selectedTeam = _selectedTeam?.Id == team.Id ? null : team;
        }

        private string TeamRowClass(AvailableTeamForLeagueDTO team, int _)
            => _selectedTeam?.Id == team.Id ? "picker-row picker-row-selected" : "picker-row";

        private async Task Save()
        {
            if (_selectedTeam is null || !_registrationDate.HasValue) return;

            _saving = true;
            try
            {
                var command = new RegisterTeamInLeagueCommand
                {
                    LeagueId = LeagueId,
                    TeamId = _selectedTeam.Id,
                    RegistrationDate = _registrationDate.Value
                };

                var response = await LeagueService.RegisterTeamInLeague(LeagueId, command);
                if (response.IsSuccess)
                {
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to register team.", Severity.Error);
                }
            }
            finally
            {
                _saving = false;
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
