using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages
{
    public partial class Home : PermissionBaseComponent
    {
        [Inject] private ILeagueService LeagueService { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        private List<FeaturedLeagueRoundDTO> _featuredLeagues = new();
        private readonly Dictionary<Guid, string> _matchLogos = new();
        private bool _loadingFeatured = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadFeaturedMatches();
        }

        private async Task LoadFeaturedMatches()
        {
            try
            {
                var response = await LeagueService.GetFeaturedLeagueMatches();
                if (response.IsSuccess)
                {
                    _featuredLeagues = response.Data ?? new();
                    BuildLogoCache();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading featured matches: {ex.Message}");
            }
            finally
            {
                _loadingFeatured = false;
            }
        }

        private void BuildLogoCache()
        {
            foreach (var league in _featuredLeagues)
            foreach (var match in league.Matches)
            {
                if (!_matchLogos.ContainsKey(match.HomeTeamId)
                    && match.HomeTeamLogoBytes?.Length > 0
                    && !string.IsNullOrEmpty(match.HomeTeamLogoContentType))
                {
                    _matchLogos[match.HomeTeamId] =
                        $"data:{match.HomeTeamLogoContentType};base64,{Convert.ToBase64String(match.HomeTeamLogoBytes)}";
                }

                if (!_matchLogos.ContainsKey(match.AwayTeamId)
                    && match.AwayTeamLogoBytes?.Length > 0
                    && !string.IsNullOrEmpty(match.AwayTeamLogoContentType))
                {
                    _matchLogos[match.AwayTeamId] =
                        $"data:{match.AwayTeamLogoContentType};base64,{Convert.ToBase64String(match.AwayTeamLogoBytes)}";
                }
            }
        }

        private string? GetLogo(Guid teamId) => _matchLogos.GetValueOrDefault(teamId);

        private async Task ScrollRow(string id, int delta)
        {
            await JS.InvokeVoidAsync("hrk.scrollFeatured", id, delta);
        }

        private static string MatchStatusLabel(MatchStatus status) => status switch
        {
            MatchStatus._2 => "Final",
            MatchStatus._3 => "Cancelled",
            MatchStatus._1 => "Rescheduled",
            _ => "Upcoming"
        };

        private static Color MatchStatusColor(MatchStatus status) => status switch
        {
            MatchStatus._2 => Color.Success,
            MatchStatus._3 => Color.Error,
            MatchStatus._1 => Color.Warning,
            _ => Color.Default
        };
    }
}
