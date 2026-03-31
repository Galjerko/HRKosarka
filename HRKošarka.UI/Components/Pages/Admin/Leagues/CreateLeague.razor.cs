using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Leagues
{
    public partial class CreateLeague : PermissionBaseComponent
    {
        [Inject] private ILeagueService LeagueService { get; set; } = default!;
        [Inject] private ISeasonService SeasonService { get; set; } = default!;
        [Inject] private IAgeCategoryService AgeCategoryService { get; set; } = default!;

        private CreateLeagueCommand _model = new();
        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _isLoading = false;
        private MudForm _form = default!;
        private bool _isFormValid = false;
        private List<SeasonDTO> _seasons = new();
        private List<AgeCategoryDTO> _ageCategories = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (!CurrentUser!.IsInRole("Administrator"))
            {
                Snackbar.Add("You need Administrator privileges to create leagues.", Severity.Warning);
                NavigationManager.NavigateTo("/leagues");
                return;
            }

            await LoadDropdowns();
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
            _isLoading = true;

            try
            {
                var response = await LeagueService.CreateLeague(_model);
                if (response.IsSuccess)
                {
                    Snackbar.Add("League created successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/leagues");
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
                        Snackbar.Add(response.Message ?? "Failed to create league.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred. Please try again.", Severity.Error);
                Console.WriteLine($"Error creating league: {ex.Message}");
            }
            finally { _isLoading = false; }
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
