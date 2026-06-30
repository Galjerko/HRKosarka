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
        [Inject] private ITeamService TeamService { get; set; } = default!;

        private MatchWithStatsDTO? _match;
        private List<TeamRepMembershipDTO> _myRepMemberships = new();
        private List<PlayerStatViewModel> _homeStats = new();
        private List<PlayerStatViewModel> _awayStats = new();
        private List<QuarterEntry> _quarters = new();
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
            !_homeStats.Any(s => !PlayerValid(s)) &&
            QuarterSumsMatchScore &&
            _homeStats.Count(s => s.IsStarter) == 5;

        private bool _canEditAwayStats =>
            !(_match?.IsResultConfirmed ?? true) && !_isDisputed &&
            (_isAdmin || (_isAwayManager && _match?.ResultSubmissionStatus == ResultSubmissionStatus._1));

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
        private int HomeQuarterSum => _quarters.Sum(q => q.Home);
        private int AwayQuarterSum => _quarters.Sum(q => q.Away);
        private bool QuarterSumsMatchScore => HomeQuarterSum == _homeScore && AwayQuarterSum == _awayScore;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isAdmin = CurrentUser?.IsInRole("Administrator") ?? false;

            if (!_isAdmin && CurrentUser != null)
            {
                var repResponse = await TeamService.GetMyRepresentativeships();
                if (repResponse.IsSuccess)
                    _myRepMemberships = repResponse.Data ?? new();
            }

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
            _isHomeManager = !_isAdmin && (CurrentPermissions.CanEdit ||
                CurrentPermissions.ManagedClubId == _match.HomeTeamClubId ||
                _myRepMemberships.Any(m => m.TeamId == _match.HomeTeamId));

            await SetClubPermissions(_match.AwayTeamClubId);
            _isAwayManager = !_isAdmin && (CurrentPermissions.CanEdit ||
                CurrentPermissions.ManagedClubId == _match.AwayTeamClubId ||
                _myRepMemberships.Any(m => m.TeamId == _match.AwayTeamId));

            _homeStats = _match.HomeTeamStats?.Select(s => new PlayerStatViewModel(s)).ToList() ?? new();
            _awayStats = _match.AwayTeamStats?.Select(s => new PlayerStatViewModel(s)).ToList() ?? new();
            _quarters = ParseQuarters(_match.QuarterResults);
        }

        private void OnDnpChanged(PlayerStatViewModel stat, bool isDnp)
        {
            stat.DidNotPlay = isDnp;
            if (isDnp) { stat.Points = 0; stat.ThreePointers = 0; stat.Fouls = 0; stat.IsStarter = false; }
        }

        private static void ToggleStarter(List<PlayerStatViewModel> stats, PlayerStatViewModel stat)
        {
            if (stat.DidNotPlay) return;
            if (!stat.IsStarter && stats.Count(s => s.IsStarter) >= 5) return;
            stat.IsStarter = !stat.IsStarter;
        }

        private static List<PlayerStatViewModel> CloneStats(List<PlayerStatViewModel> source) =>
            source.Select(s => new PlayerStatViewModel
            {
                PlayerId = s.PlayerId,
                PlayerName = s.PlayerName,
                JerseyNumber = s.JerseyNumber,
                Position = s.Position,
                Points = s.Points,
                ThreePointers = s.ThreePointers,
                Fouls = s.Fouls,
                DidNotPlay = s.DidNotPlay,
                IsStarter = s.IsStarter,
                StatsEntered = s.StatsEntered
            }).ToList();

        private async Task<bool> SaveStatsCore(Guid teamId, List<PlayerStatViewModel> stats)
        {
            if (_match is null) return false;
            var command = new SaveMatchStatsCommand
            {
                TeamId = teamId,
                HomeScore = _homeScore,
                AwayScore = _awayScore,
                QuarterResults = FormatQuarters(_quarters),
                PlayerStats = stats.Select(s => new PlayerStatEntry
                {
                    PlayerId = s.PlayerId,
                    Points = s.Points,
                    ThreePointers = s.ThreePointers,
                    Fouls = s.Fouls,
                    DidNotPlay = s.DidNotPlay,
                    IsStarter = s.IsStarter
                }).ToList()
            };
            var response = await MatchService.SaveMatchStats(_match.Id, command);
            if (!response.IsSuccess)
                Snackbar.Add(response.Message ?? "Failed to save stats.", Severity.Error);
            return response.IsSuccess;
        }

        // Non-admin: save one team, reload both panels
        private async Task SaveStats(Guid teamId, List<PlayerStatViewModel> stats)
        {
            if (_match is null) return;
            var starterCount = stats.Count(s => s.IsStarter);
            if (starterCount != 5)
            {
                Snackbar.Add($"Select exactly 5 starters before saving ({starterCount} selected).", Severity.Error);
                return;
            }
            _isSaving = true;
            try
            {
                if (await SaveStatsCore(teamId, stats))
                {
                    Snackbar.Add("Stats saved.", Severity.Success);
                    await LoadMatch();
                }
            }
            finally { _isSaving = false; }
        }

        // Admin: save home stats, preserve away panel's local edits
        private async Task SaveHomeStatsDraft()
        {
            if (_match is null) return;
            var starterCount = _homeStats.Count(s => s.IsStarter);
            if (starterCount != 5)
            {
                Snackbar.Add($"Select exactly 5 home starters before saving ({starterCount} selected).", Severity.Error);
                return;
            }
            _isSaving = true;
            try
            {
                if (await SaveStatsCore(_match.HomeTeamId, _homeStats))
                {
                    Snackbar.Add("Home stats saved.", Severity.Success);
                    var awaySnapshot = CloneStats(_awayStats);
                    await LoadMatch();
                    _awayStats = awaySnapshot;
                }
            }
            finally { _isSaving = false; }
        }

        // Admin: save away stats, preserve home panel's local edits
        private async Task SaveAwayStatsDraft()
        {
            if (_match is null) return;
            var starterCount = _awayStats.Count(s => s.IsStarter);
            if (starterCount != 5)
            {
                Snackbar.Add($"Select exactly 5 away starters before saving ({starterCount} selected).", Severity.Error);
                return;
            }
            _isSaving = true;
            try
            {
                if (await SaveStatsCore(_match.AwayTeamId, _awayStats))
                {
                    Snackbar.Add("Away stats saved.", Severity.Success);
                    var homeSnapshot = CloneStats(_homeStats);
                    await LoadMatch();
                    _homeStats = homeSnapshot;
                }
            }
            finally { _isSaving = false; }
        }

        // Admin: save both panels, then reload
        private async Task SaveAllDraft()
        {
            if (_match is null) return;
            var homeStarters = _homeStats.Count(s => s.IsStarter);
            var awayStarters = _awayStats.Count(s => s.IsStarter);
            if (homeStarters != 5 || awayStarters != 5)
            {
                Snackbar.Add($"Both teams need 5 starters selected (home: {homeStarters}, away: {awayStarters}).", Severity.Error);
                return;
            }
            _isSaving = true;
            try
            {
                var homeOk = await SaveStatsCore(_match.HomeTeamId, _homeStats);
                var awayOk = await SaveStatsCore(_match.AwayTeamId, _awayStats);
                if (homeOk || awayOk)
                {
                    Snackbar.Add("All stats saved as draft.", Severity.Success);
                    await LoadMatch();
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
            if (!QuarterSumsMatchScore)
            {
                Snackbar.Add($"Cannot submit: quarter totals ({HomeQuarterSum}:{AwayQuarterSum}) do not match the score ({_homeScore}:{_awayScore}).", Severity.Error);
                return;
            }
            var homeStarterCount = _homeStats.Count(s => s.IsStarter);
            if (homeStarterCount != 5)
            {
                Snackbar.Add($"Cannot submit: exactly 5 starters must be selected ({homeStarterCount} currently marked).", Severity.Error);
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
                var errors = response.Errors?.Any() == true
                    ? response.Errors
                    : new List<string> { response.Message ?? "Failed to send proposal." };
                foreach (var error in errors)
                    Snackbar.Add(error, Severity.Error);
            }
        }

        private void AddOtPeriod() => _quarters.Add(new QuarterEntry());

        private void RemoveLastOtPeriod()
        {
            if (_quarters.Count > 4)
                _quarters.RemoveAt(_quarters.Count - 1);
        }

        private static List<QuarterEntry> ParseQuarters(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return new List<QuarterEntry> { new(), new(), new(), new() };
            return raw.Split(';').Select(seg =>
            {
                var parts = seg.Split(':');
                return new QuarterEntry
                {
                    Home = int.TryParse(parts.ElementAtOrDefault(0), out var h) ? h : 0,
                    Away = int.TryParse(parts.ElementAtOrDefault(1), out var a) ? a : 0
                };
            }).ToList();
        }

        private static string FormatQuarters(List<QuarterEntry> quarters)
            => string.Join(";", quarters.Select(q => $"{q.Home}:{q.Away}"));

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

    public class QuarterEntry
    {
        public int Home { get; set; }
        public int Away { get; set; }
    }

    public class PlayerStatViewModel
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public int Position { get; set; }
        public int Points { get; set; }
        public int ThreePointers { get; set; }
        public int Fouls { get; set; }
        public bool DidNotPlay { get; set; }
        public bool IsStarter { get; set; }
        public bool StatsEntered { get; set; }

        public PlayerStatViewModel() { }

        public PlayerStatViewModel(PlayerMatchStatDTO dto)
        {
            PlayerId = dto.PlayerId;
            PlayerName = dto.PlayerName ?? string.Empty;
            JerseyNumber = dto.JerseyNumber;
            Position = dto.Position;
            Points = dto.Points;
            ThreePointers = dto.ThreePointers;
            Fouls = dto.Fouls;
            DidNotPlay = dto.DidNotPlay;
            IsStarter = dto.IsStarter;
            StatsEntered = dto.StatsEntered;
        }
    }
}
