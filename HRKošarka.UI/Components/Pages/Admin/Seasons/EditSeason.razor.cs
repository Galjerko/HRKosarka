using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Seasons
{
    public partial class EditSeason : PermissionBaseComponent
    {
        [Inject] private ISeasonService SeasonService { get; set; } = default!;
        [Parameter] public Guid Id { get; set; }

        private readonly UpdateSeasonCommand _command = new();
        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private MudForm _form = default!;
        private bool _isFormValid = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSeason();
        }

        private async Task LoadSeason()
        {
            _isLoading = true;
            try
            {
                var result = await SeasonService.GetSeasonById(Id);

                if (result.IsSuccess && result.Data != null)
                {
                    var season = result.Data;
                    _command.Id = season.Id;
                    _command.Name = season.Name;
                    _command.IsActive = season.IsActive;
                    _startDate = season.StartDate.DateTime;
                    _endDate = season.EndDate.DateTime;
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Season not found.", Severity.Error);
                    NavigationManager.NavigateTo("/seasons");
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"An unexpected error occurred while loading season: {ex.Message}", Severity.Error);
                NavigationManager.NavigateTo("/seasons");
            }
            finally { _isLoading = false; }
        }

        private async Task HandleSubmit()
        {
            await _form.Validate();
            if (!_form.IsValid)
            { Snackbar.Add("Please fix the validation errors before submitting.", Severity.Warning); return; }

            if (_startDate == null || _endDate == null)
            { Snackbar.Add("Please select both start and end dates.", Severity.Warning); return; }

            _command.StartDate = _startDate.Value;
            _command.EndDate = _endDate.Value;
            _isProcessing = true;

            try
            {
                var result = await SeasonService.UpdateSeason(Id, _command);

                if (!result.IsSuccess)
                { Snackbar.Add(result.Message ?? "Failed to update season.", Severity.Error); return; }

                Snackbar.Add("Season updated successfully.", Severity.Success);
                NavigationManager.NavigateTo("/seasons");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"An unexpected error occurred: {ex.Message}", Severity.Error);
            }
            finally { _isProcessing = false; }
        }
    }
}
