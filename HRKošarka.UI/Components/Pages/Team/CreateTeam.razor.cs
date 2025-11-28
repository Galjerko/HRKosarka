using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Team
{
    public partial class CreateTeam
    {
        [Parameter] public Guid clubId { get; set; }
        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private IClubService ClubService { get; set; } = default!;
        [Inject] private IAgeCategoryService AgeCategoryService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private CreateTeamCommand _model = new();
        private ClubDetailsDTO? _club;
        private List<AgeCategoryDTO>? _ageCategories;
        private MudForm _form = default!;
        private bool _isFormValid = false;
        private bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            try
            {
                var clubResp = await ClubService.GetClubDetails(clubId);
                if (!clubResp.IsSuccess || clubResp.Data == null)
                {
                    _club = null;
                    Snackbar.Add("Failed to load club details.", Severity.Error);
                    return;
                }
                _club = clubResp.Data;
                _model.ClubId = clubId;

                var acResp = await AgeCategoryService.GetAllAgeCategories();
                _ageCategories = acResp.Data ?? new List<AgeCategoryDTO>();

                if (_ageCategories.Any())
                {
                    _model.AgeCategoryId = _ageCategories.First().Id;
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An error occurred while loading data.", Severity.Error);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task HandleSubmit()
        {
            await _form.Validate();
            if (!_isFormValid)
            {
                Snackbar.Add("Please fix the validation errors before submitting.", Severity.Warning);
                return;
            }

            _isLoading = true;
            try
            {
                var response = await TeamService.CreateTeam(_model);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Team created successfully!", Severity.Success);
                    NavigationManager.NavigateTo($"/clubs/{_club.Id}");
                }
                else
                {
                    if (response.Errors?.Any() == true)
                        foreach (var error in response.Errors)
                            Snackbar.Add(error, Severity.Error);
                    Snackbar.Add(response.Message ?? "Failed to create team.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("Unexpected error while creating team.", Severity.Error);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _isLoading = false;
            }
        }

        // Validation helper for team name (feel free to add more)
        private IEnumerable<string> ValidateTeamName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                yield return "Team name is required";
            else if (name.Length > 150)
                yield return "Team name must not exceed 150 characters";
        }
    }
}