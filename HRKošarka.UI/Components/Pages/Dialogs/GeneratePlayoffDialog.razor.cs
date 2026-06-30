using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class GeneratePlayoffDialog
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Guid LeagueId { get; set; }
        [Parameter] public int PlayoffTeamCount { get; set; } // 2, 4, or 8
        [Parameter] public DateTime? MinStartDate { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private List<RoundConfig> _rounds = new();
        private bool _include3rdPlace = false;
        private bool _saving = false;
        private DateTime? _playoffStartDate;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _playoffStartDate = MinStartDate ?? DateTime.Today;
            _rounds = BuildRoundConfigs(PlayoffTeamCount);
        }

        private static List<RoundConfig> BuildRoundConfigs(int teamCount)
        {
            return teamCount switch
            {
                8 => new List<RoundConfig>
                {
                    new("Quarter-Final", 3),
                    new("Semi-Final", 3),
                    new("Final", 4)
                },
                4 => new List<RoundConfig>
                {
                    new("Semi-Final", 3),
                    new("Final", 4)
                },
                _ => new List<RoundConfig>
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
                    LeagueId = LeagueId,
                    PlayoffStartDate = _playoffStartDate.Value,
                    RoundWinsNeeded = _rounds.Select(r => r.WinsNeeded).ToList(),
                    Include3rdPlace = _include3rdPlace
                };

                var response = await LeagueService.GeneratePlayoff(LeagueId, command);
                if (response.IsSuccess)
                {
                    MudDialog.Close(DialogResult.Ok(true));
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

        private void Cancel() => MudDialog.Cancel();

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
