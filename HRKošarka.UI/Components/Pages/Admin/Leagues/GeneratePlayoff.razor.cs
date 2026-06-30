using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Leagues
{
    public partial class GeneratePlayoff : HRKošarka.UI.Components.Base.PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private LeagueDetailsDTO? _league;
        private List<RoundConfig> _rounds = new();
        private bool _include3rdPlace = false;
        private bool _saving = false;
        private bool _loading = true;
        private DateTime? _playoffStartDate;
        private DateTime? _minStartDate;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var leagueTask = LeagueService.GetLeagueById(Id);
            var scheduleTask = LeagueService.GetLeagueSchedule(Id);
            await Task.WhenAll(leagueTask, scheduleTask);

            var leagueResponse = await leagueTask;
            if (!leagueResponse.IsSuccess || leagueResponse.Data is null)
            {
                NavigationManager.NavigateTo($"/leagues/{Id}");
                return;
            }
            _league = leagueResponse.Data;

            var scheduleResponse = await scheduleTask;
            if (scheduleResponse.IsSuccess && scheduleResponse.Data?.Any() == true)
            {
                var lastDate = scheduleResponse.Data
                    .Max(r => r.ScheduledDate)
                    .Date;
                _minStartDate = lastDate.AddDays(1);
            }
            else
            {
                _minStartDate = _league.StartDate.Date.AddDays(1);
            }

            _playoffStartDate = _minStartDate;
            _rounds = BuildRoundConfigs(_league.PlayoffTeamCount ?? 4);
            _include3rdPlace = _league.PlayoffHas3rdPlace;
            _loading = false;
        }

        private static List<RoundConfig> BuildRoundConfigs(int teamCount)
        {
            return teamCount switch
            {
                8 => new()
                {
                    new("Quarter-Final", 3),
                    new("Semi-Final", 3),
                    new("Final", 4)
                },
                4 => new()
                {
                    new("Semi-Final", 3),
                    new("Final", 4)
                },
                _ => new()
                {
                    new("Final", 4)
                }
            };
        }

        private async Task Submit()
        {
            if (_playoffStartDate == null)
            {
                Snackbar.Add("Please select a playoff start date.", Severity.Warning);
                return;
            }

            _saving = true;
            try
            {
                var command = new GeneratePlayoffCommand
                {
                    LeagueId = Id,
                    PlayoffStartDate = _playoffStartDate.Value,
                    RoundWinsNeeded = _rounds.Select(r => r.WinsNeeded).ToList(),
                    Include3rdPlace = _include3rdPlace
                };

                var response = await LeagueService.GeneratePlayoff(Id, command);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Playoff bracket generated!", Severity.Success);
                    NavigationManager.NavigateTo($"/leagues/{Id}");
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to generate playoff.", Severity.Error);
                    if (response.Errors?.Any() == true)
                        foreach (var err in response.Errors)
                            Snackbar.Add(err, Severity.Warning);
                }
            }
            finally
            {
                _saving = false;
            }
        }

        private class RoundConfig
        {
            public string RoundName { get; set; }
            public int WinsNeeded { get; set; }

            public RoundConfig(string roundName, int winsNeeded)
            {
                RoundName = roundName;
                WinsNeeded = winsNeeded;
            }
        }
    }
}
