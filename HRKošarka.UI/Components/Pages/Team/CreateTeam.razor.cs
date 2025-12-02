using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Team
{
    public partial class CreateTeam : PermissionBaseComponent
    {
        [Parameter] public Guid clubId { get; set; }
        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private IClubService ClubService { get; set; } = default!;
        [Inject] private IAgeCategoryService AgeCategoryService { get; set; } = default!;

        private CreateTeamCommand _model = new();
        private ClubDetailsDTO? _club;
        private List<AgeCategoryDTO>? _ageCategories;
        private MudForm _form = default!;
        private bool _isFormValid = false;
        private bool _isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
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
                await SetClubPermissions(clubId);

                if (!CurrentPermissions.CanCreate)
                {
                    Snackbar.Add("You don't have permission to create teams for this club.", Severity.Warning);
                    NavigationManager.NavigateTo($"/clubs/{clubId}");
                    return;
                }

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
                    NavigationManager.NavigateTo($"/clubs/{_club!.Id}");
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

        private IEnumerable<string> ValidateTeamName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                yield return "Team name is required";
            }
            else if (name.Length > 150)
            {
                yield return "Team name must not exceed 150 characters";
            }
        }
    }
}
