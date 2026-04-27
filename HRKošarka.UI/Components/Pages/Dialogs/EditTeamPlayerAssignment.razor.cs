using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class EditTeamPlayerAssignment
    {
        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public TeamRosterPlayerDTO? Player { get; set; }

        private int? _jerseyNumber;
        private bool _saving;

        protected override void OnInitialized()
        {
            _jerseyNumber = Player?.JerseyNumber;
        }

        private void Cancel() => MudDialog.Cancel();

        private void Save()
        {
            _saving = true;
            MudDialog.Close(DialogResult.Ok(new UpdatePlayerAssignmentInTeamCommand
            {
                TeamId = Guid.Empty,
                PlayerId = Guid.Empty,
                JerseyNumber = _jerseyNumber
            }));
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
