using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Matches
{
    public partial class MatchDetails : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private IMatchService MatchService { get; set; } = default!;

        private MatchWithStatsDTO? _match;
        private List<PlayerStatViewModel> _homeStats = new();
        private List<PlayerStatViewModel> _awayStats = new();
        private int _homeScore;
        private int _awayScore;
        private bool _isLoading = true;
        private bool _isSaving = false;
        private bool _isConfirming = false;
        private bool _isAdmin = false;
        private bool _isHomeManager = false;
        private bool _isAwayManager = false;

        // Dispute inline form
        private bool _showDisputeForm = false;
        private string _disputeReason = string.Empty;

        // Forfeit dialogs
        private bool _showForfeitHomeDialog = false;
        private bool _showForfeitAwayDialog = false;

        // Reschedule proposal form
        private bool _showProposeForm = false;
        private DateTime? _proposedDate;
        private TimeSpan? _proposedTime = TimeSpan.FromHours(19);
        private string _proposeReason = string.Empty;
        private DateTime _rescheduleMinDate => _match == null
            ? DateTime.Today.AddDays(1)
            : new DateTime(Math.Max(DateTime.Today.AddDays(1).Ticks, _match.LeagueStartDate.Ticks));
        private DateTime _rescheduleMaxDate => _match?.LeagueEndDate ?? DateTime.Today.AddYears(1);

        // Reset confirm dialog
        private bool _showResetDialog = false;

        // Venue edit
        private bool _showVenueEdit = false;
        private string _venueEdit = string.Empty;

        private bool _isDisputed =>
            _match?.ResultSubmissionStatus == HRKošarka.UI.Services.Base.ResultSubmissionStatus._3;

        private bool _canEditScore =>
            !(_match?.IsResultConfirmed ?? true) && !_isDisputed &&
            (_isAdmin || (_isHomeManager && _match?.ResultSubmissionStatus == ResultSubmissionStatus._0));

        private bool _canSubmitHome =>
            _canEditScore && _isHomeManager &&
            _homeStats.Any() && HomeScoreMatchesStats &&
            !_homeStats.Any(s => !PlayerValid(s));

        private bool _canEditAwayStats =>
            !(_match?.IsResultConfirmed ?? true) && !_isDisputed && (_isAdmin || _isAwayManager);

        private bool _canRespondToProposal =>
            _match?.PendingReschedule != null &&
            ((_isHomeManager && _match.PendingReschedule.ProposerIsHome == false) ||
             (_isAwayManager && _match.PendingReschedule.ProposerIsHome == true));

        private bool _isProposer =>
            _match?.PendingReschedule != null &&
            ((_isHomeManager && _match.PendingReschedule.ProposerIsHome == true) ||
             (_isAwayManager && _match.PendingReschedule.ProposerIsHome == false));

        // Totals
        private int HomeTotalPts   => _homeStats.Where(s => !s.DidNotPlay).Sum(s => s.Points);
        private int HomeTotalThree => _homeStats.Where(s => !s.DidNotPlay).Sum(s => s.ThreePointers);
        private int HomeTotalFouls => _homeStats.Where(s => !s.DidNotPlay).Sum(s => s.Fouls);
        private int AwayTotalPts   => _awayStats.Where(s => !s.DidNotPlay).Sum(s => s.Points);
        private int AwayTotalThree => _awayStats.Where(s => !s.DidNotPlay).Sum(s => s.ThreePointers);
        private int AwayTotalFouls => _awayStats.Where(s => !s.DidNotPlay).Sum(s => s.Fouls);

        // Validation
        private bool HomeScoreMatchesStats => HomeTotalPts == _homeScore;
        private bool AwayScoreMatchesStats => AwayTotalPts == _awayScore;
        private bool PlayerValid(PlayerStatViewModel s) => s.DidNotPlay || s.Points >= s.ThreePointers * 3;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isAdmin = CurrentUser?.IsInRole("Administrator") ?? false;
            await LoadMatch();
            _isLoading = false;
        }

        private async Task LoadMatch()
        {
            var response = await MatchService.GetMatchWithStats(Id);
            if (!response.IsSuccess || response.Data == null) return;

            _match = response.Data;
            _homeScore = _match.HomeScore ?? 0;
            _awayScore = _match.AwayScore ?? 0;

            await SetClubPermissions(_match.HomeTeamClubId);
            _isHomeManager = !_isAdmin && (CurrentPermissions.CanEdit || CurrentPermissions.ManagedClubId == _match.HomeTeamClubId);

            await SetClubPermissions(_match.AwayTeamClubId);
            _isAwayManager = !_isAdmin && (CurrentPermissions.CanEdit || CurrentPermissions.ManagedClubId == _match.AwayTeamClubId);

            _homeStats = _match.HomeTeamStats?.Select(s => new PlayerStatViewModel(s)).ToList() ?? new();
            _awayStats = _match.AwayTeamStats?.Select(s => new PlayerStatViewModel(s)).ToList() ?? new();
        }

        private void OnDnpChanged(PlayerStatViewModel stat, bool isDnp)
        {
            stat.DidNotPlay = isDnp;
            if (isDnp) { stat.Points = 0; stat.ThreePointers = 0; stat.Fouls = 0; }
        }

        private async Task SaveStats(Guid teamId, List<PlayerStatViewModel> stats)
        {
            if (_match is null) return;
            _isSaving = true;
            try
            {
                var command = new SaveMatchStatsCommand
                {
                    TeamId = teamId,
                    HomeScore = _homeScore,
                    AwayScore = _awayScore,
                    PlayerStats = stats.Select(s => new PlayerStatEntry
                    {
                        PlayerId = s.PlayerId,
                        Points = s.Points,
                        ThreePointers = s.ThreePointers,
                        Fouls = s.Fouls,
                        DidNotPlay = s.DidNotPlay
                    }).ToList()
                };
                var response = await MatchService.SaveMatchStats(_match.Id, command);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Stats saved.", Severity.Success);
                    await LoadMatch();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to save stats.", Severity.Error);
                }
            }
            finally { _isSaving = false; }
        }

        private async Task SaveVenue()
        {
            if (_match is null) return;
            var response = await MatchService.UpdateMatchVenue(_match.Id, new UpdateMatchVenueCommand
            {
                Venue = _venueEdit
            });
            if (response.IsSuccess)
            {
                Snackbar.Add("Venue updated.", Severity.Success);
                _showVenueEdit = false;
                await LoadMatch();
            }
            else
            {
                Snackbar.Add(response.Message ?? "Failed to update venue.", Severity.Error);
            }
        }

        private async Task SubmitHome()
        {
            if (_match is null) return;

            if (_homeStats.Any() && HomeTotalPts != _homeScore)
            {
                Snackbar.Add($"Cannot submit: {_match.HomeTeamName} player totals ({HomeTotalPts} pts) do not match the score ({_homeScore} pts).", Severity.Error);
                return;
            }
            if (_homeStats.Any(s => !PlayerValid(s)))
            {
                Snackbar.Add("Cannot submit: one or more players have fewer total points than their three-pointers alone would give (PTS < 3PT × 3).", Severity.Error);
                return;
            }

            _isSaving = true;
            try
            {
                var response = await MatchService.SubmitHomeStats(_match.Id);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Stats submitted to away team for confirmation.", Severity.Success);
                    await LoadMatch();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to submit.", Severity.Error);
                }
            }
            finally { _isSaving = false; }
        }

        private async Task ConfirmResult()
        {
            if (_match is null) return;

            if (_homeStats.Any() && HomeTotalPts != _homeScore)
            {
                Snackbar.Add($"Cannot confirm: {_match.HomeTeamName} player totals ({HomeTotalPts} pts) do not match the score ({_homeScore} pts).", Severity.Error);
                return;
            }
            if (_awayStats.Any() && AwayTotalPts != _awayScore)
            {
                Snackbar.Add($"Cannot confirm: {_match.AwayTeamName} player totals ({AwayTotalPts} pts) do not match the score ({_awayScore} pts).", Severity.Error);
                return;
            }
            if (_homeStats.Any(s => !PlayerValid(s)) || _awayStats.Any(s => !PlayerValid(s)))
            {
                Snackbar.Add("Cannot confirm: one or more players have fewer total points than their three-pointers alone would give (PTS < 3PT × 3).", Severity.Error);
                return;
            }

            _isConfirming = true;
            try
            {
                var response = await MatchService.ConfirmMatchResult(_match.Id);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Result confirmed!", Severity.Success);
                    await LoadMatch();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to confirm.", Severity.Error);
                }
            }
            finally { _isConfirming = false; }
        }

        private async Task SubmitDispute()
        {
            if (_match is null) return;
            if (string.IsNullOrWhiteSpace(_disputeReason))
            {
                Snackbar.Add("Please enter a reason for the dispute.", Severity.Warning);
                return;
            }

            var response = await MatchService.DisputeMatchResult(_match.Id, new DisputeMatchResultCommand
            {
                Reason = _disputeReason
            });

            if (response.IsSuccess)
            {
                Snackbar.Add("Result disputed. Admin will review.", Severity.Warning);
                _showDisputeForm = false;
                _disputeReason = string.Empty;
                await LoadMatch();
            }
            else
            {
                Snackbar.Add(response.Message ?? "Failed to dispute.", Severity.Error);
            }
        }

        private async Task ResetResult()
        {
            if (_match is null) return;
            _showResetDialog = false;
            var response = await MatchService.ResetMatchResult(_match.Id);
            if (response.IsSuccess)
            {
                Snackbar.Add("Match result reset. Home team can re-enter.", Severity.Info);
                await LoadMatch();
            }
            else
            {
                Snackbar.Add(response.Message ?? "Failed to reset.", Severity.Error);
            }
        }

        private async Task PerformForfeit(Guid forfeitingTeamId)
        {
            if (_match is null) return;
            _showForfeitHomeDialog = false;
            _showForfeitAwayDialog = false;
            var response = await MatchService.RecordForfeit(_match.Id, new RecordForfeitCommand
            {
                ForfeitingTeamId = forfeitingTeamId
            });
            if (response.IsSuccess)
            {
                Snackbar.Add("Forfeit recorded.", Severity.Success);
                await LoadMatch();
            }
            else
            {
                Snackbar.Add(response.Message ?? "Failed to record forfeit.", Severity.Error);
            }
        }

        private async Task SubmitProposal()
        {
            if (_match is null || _proposedDate is null) return;
            if (string.IsNullOrWhiteSpace(_proposeReason))
            {
                Snackbar.Add("Please enter a reason for rescheduling.", Severity.Warning);
                return;
            }

            var proposedDateTime = _proposedDate.Value.Date.Add(_proposedTime ?? TimeSpan.FromHours(19));
            var response = await MatchService.ProposeReschedule(_match.Id, new ProposeRescheduleCommand
            {
                ProposedDate = proposedDateTime,
                Reason = _proposeReason
            });

            if (response.IsSuccess)
            {
                Snackbar.Add("Reschedule proposal sent. The other team has 48 hours to respond.", Severity.Success);
                _showProposeForm = false;
                _proposedDate = null;
                _proposedTime = TimeSpan.FromHours(19);
                _proposeReason = string.Empty;
                await LoadMatch();
            }
            else
            {
                Snackbar.Add(response.Message ?? "Failed to send proposal.", Severity.Error);
            }
        }

        private async Task RespondToProposal(bool accept)
        {
            if (_match is null) return;
            var response = await MatchService.RespondToReschedule(_match.Id, new RespondToRescheduleCommand
            {
                Accept = accept
            });

            if (response.IsSuccess)
            {
                Snackbar.Add(response.Message ?? (accept ? "Proposal accepted." : "Proposal rejected."),
                    accept ? Severity.Success : Severity.Info);
                await LoadMatch();
            }
            else
            {
                Snackbar.Add(response.Message ?? "Failed.", Severity.Error);
            }
        }
    }

    public class PlayerStatViewModel
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public int Points { get; set; }
        public int ThreePointers { get; set; }
        public int Fouls { get; set; }
        public bool DidNotPlay { get; set; }
        public bool StatsEntered { get; set; }

        public PlayerStatViewModel() { }

        public PlayerStatViewModel(PlayerMatchStatDTO dto)
        {
            PlayerId = dto.PlayerId;
            PlayerName = dto.PlayerName ?? string.Empty;
            JerseyNumber = dto.JerseyNumber;
            Points = dto.Points;
            ThreePointers = dto.ThreePointers;
            Fouls = dto.Fouls;
            DidNotPlay = dto.DidNotPlay;
            StatsEntered = dto.StatsEntered;
        }
    }
}
