using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Seasons
{
    public partial class CreateSeason : PermissionBaseComponent
    {
        [Inject] private ISeasonService SeasonService { get; set; } = default!;

        private readonly CreateSeasonCommand _command = new();
        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _isLoading = false;
        private bool _isProcessing = false;
        private MudForm _form = default!;
        private bool _isFormValid = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
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
                var result = await SeasonService.CreateSeason(_command);

                if (!result.IsSuccess)
                {
                    if (result.Errors?.Any() == true)
                        foreach (var error in result.Errors)
                            Snackbar.Add(error, Severity.Error);
                    else
                        Snackbar.Add(result.Message ?? "Failed to create season.", Severity.Error);
                    return;
                }

                Snackbar.Add("Season created successfully.", Severity.Success);
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
