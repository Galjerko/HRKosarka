using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class EditTeamPlayerAssignment
    {
        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public TeamRosterPlayerDTO? Player { get; set; }
        [Parameter] public Guid TeamId { get; set; }

        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private int? _jerseyNumber;
        private bool _saving;
        private string? _errorMessage;

        protected override void OnInitialized()
        {
            _jerseyNumber = Player?.JerseyNumber;
        }

        private void Cancel() => MudDialog.Cancel();

        private async Task Save()
        {
            if (Player == null) return;

            _saving = true;
            _errorMessage = null;
            StateHasChanged();

            try
            {
                var response = await TeamService.UpdatePlayerAssignmentInTeam(TeamId, Player.PlayerId,
                    new UpdatePlayerAssignmentInTeamCommand
                    {
                        TeamId = TeamId,
                        PlayerId = Player.PlayerId,
                        JerseyNumber = _jerseyNumber
                    });

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player assignment updated successfully.", Severity.Success);
                    MudDialog.Close(DialogResult.Ok(true));
                }
                else
                {
                    _errorMessage = response.Errors?.Any() == true
                        ? string.Join(" ", response.Errors)
                        : response.Message ?? "Failed to update assignment.";
                }
            }
            finally
            {
                _saving = false;
                StateHasChanged();
            }
        }

        private static string GetPositionLabel(HRKošarka.UI.Services.Base.Position? position) => position switch
        {
            HRKošarka.UI.Services.Base.Position._0 => "Point Guard",
            HRKošarka.UI.Services.Base.Position._1 => "Shooting Guard",
            HRKošarka.UI.Services.Base.Position._2 => "Small Forward",
            HRKošarka.UI.Services.Base.Position._3 => "Power Forward",
            HRKošarka.UI.Services.Base.Position._4 => "Center",
            _ => "-"
        };
    }
}
