using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Leagues
{
    public partial class EditLeague : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;
        [Inject] private ISeasonService SeasonService { get; set; } = default!;
        [Inject] private IAgeCategoryService AgeCategoryService { get; set; } = default!;

        private UpdateLeagueCommand _model = new();
        private LeagueDetailsDTO? _league;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private MudForm _form = default!;
        private bool _isFormValid = false;
        private List<SeasonDTO> _seasons = new();
        private List<AgeCategoryDTO> _ageCategories = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadDropdowns();
            await LoadLeagueData();
        }

        private async Task LoadDropdowns()
        {
            try
            {
                var seasonsResponse = await SeasonService.GetSeasons(
                    new HRKošarka.UI.Services.Base.Common.Requests.PaginationRequest { Page = 1, PageSize = 999 });

                if (seasonsResponse.IsSuccess && seasonsResponse.Data != null)
                {
                    _seasons = seasonsResponse.Data.ToList();
                }

                var acResponse = await AgeCategoryService.GetAllAgeCategories();

                if (acResponse.IsSuccess && acResponse.Data != null)
                {
                    _ageCategories = acResponse.Data;
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading form data.", Severity.Error);
                Console.WriteLine($"Error loading dropdowns: {ex.Message}");
            }
        }

        private async Task LoadLeagueData()
        {
            _isLoading = true;
            try
            {
                var response = await LeagueService.GetLeagueById(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _league = response.Data;
                    _startDate = _league.StartDate;
                    _endDate = _league.EndDate;
                    _model = new UpdateLeagueCommand
                    {
                        Id = _league.Id,
                        Name = _league.Name ?? string.Empty,
                        SeasonId = _league.SeasonId,
                        AgeCategoryId = _league.AgeCategoryId,
                        Gender = _league.Gender,
                        CompetitionType = _league.CompetitionType,
                        NumberOfRounds = _league.NumberOfRounds,
                        IsActive = _league.IsActive,
                        StartDate = _league.StartDate,
                        EndDate = _league.EndDate
                    };
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to load league details.", Severity.Error);
                    }

                    NavigationManager.NavigateTo("leagues");
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading league details.", Severity.Error);
                Console.WriteLine($"Error loading league details: {ex.Message}");
                NavigationManager.NavigateTo("/leagues");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            await _form.Validate();

            if (!_form.IsValid)
            {
                Snackbar.Add("Please fix the validation errors before submitting.", Severity.Warning);
                return;
            }

            if (_startDate == null || _endDate == null)
            {
                Snackbar.Add("Please select both start and end dates.", Severity.Warning);
                return;
            }

            if (_endDate <= _startDate)
            {
                Snackbar.Add("End date must be after start date.", Severity.Warning);
                return;
            }

            _model.StartDate = _startDate.Value;
            _model.EndDate = _endDate.Value;
            _isProcessing = true;

            try
            {
                var response = await LeagueService.UpdateLeague(Id, _model);

                if (response.IsSuccess)
                {
                    Snackbar.Add("League updated successfully!", Severity.Success);
                    NavigationManager.NavigateTo($"/admin/leagues/{Id}");
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to update league.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred. Please try again.", Severity.Error);
                Console.WriteLine($"Error updating league: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private IEnumerable<string> ValidateLeagueName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                yield return "League name is required";
            }
            else if (name.Length > 200)
            {
                yield return "League name must not exceed 200 characters";
            }
        }
    }
}
