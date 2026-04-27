using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Player
{
    public partial class EditPlayer /*: PermissionBaseComponent*/
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private IPlayerService PlayerService { get; set; } = default!;
        [Inject] private IConfiguration Configuration { get; set; } = default!;

        private UpdatePlayerCommand _model = new();
        private PlayerDetailsDTO? _player;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private DateTime? _dateOfBirth;
        private MudForm _form = default!;
        private bool _isFormValid = false;
        private List<string> _countries = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _countries = Configuration.GetSection("Countries").Get<List<string>>() ?? new List<string>();
            await LoadPlayerData();
        }

        private async Task LoadPlayerData()
        {
            _isLoading = true;

            try
            {
                var response = await PlayerService.GetPlayerDetails(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _player = response.Data;

                    if (!_player.IsActive)
                    {
                        Snackbar.Add("Cannot edit inactive player.", Severity.Warning);
                        NavigationManager.NavigateTo($"/players/{Id}");
                        return;
                    }

                    _dateOfBirth = _player.DateOfBirth;

                    _model = new UpdatePlayerCommand
                    {
                        Id = _player.Id,
                        FirstName = _player.FirstName,
                        LastName = _player.LastName,
                        RegistrationNumber = _player.RegistrationNumber,
                        DateOfBirth = _player.DateOfBirth,
                        Height = _player.Height,
                        Weight = _player.Weight,
                        Position = _player.Position,
                        Nationality = _player.Nationality ?? string.Empty,
                    };
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                            Snackbar.Add(error, Severity.Error);
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to load player details", Severity.Error);
                    }

                    NavigationManager.NavigateTo("/players");
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading player details.", Severity.Error);
                Console.WriteLine($"Error loading player details: {ex.Message}");
                NavigationManager.NavigateTo("/players");
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

            _isProcessing = true;

            try
            {
                if (_dateOfBirth.HasValue)
                {
                    _model.DateOfBirth = _dateOfBirth.Value;
                }

                var response = await PlayerService.UpdatePlayer(Id, _model);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player updated successfully!", Severity.Success);
                    NavigationManager.NavigateTo($"/players/{Id}");
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                            Snackbar.Add(error, Severity.Error);
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to update player", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred. Please try again.", Severity.Error);
                Console.WriteLine($"Error updating player: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void OnImageChanged((string? name, string? contentType, byte[]? bytes) image)
        {
            _model.ImageName = image.name;
            _model.ImageContentType = image.contentType;
            _model.ImageBytes = image.bytes;
        }

        private IEnumerable<string> ValidateFirstName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                yield return "First name is required";
            else if (name.Length > 100)
                yield return "First name must not exceed 100 characters";
        }

        private IEnumerable<string> ValidateLastName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                yield return "Last name is required";
            else if (name.Length > 100)
                yield return "Last name must not exceed 100 characters";
        }

        private IEnumerable<string> ValidateRegistrationNumber(string regNo)
        {
            if (string.IsNullOrWhiteSpace(regNo))
                yield return "Registration number is required";
        }

        private IEnumerable<string> ValidateDateOfBirth(DateTime? date)
        {
            if (date == null)
                yield return "Date of birth is required";
            else if (date >= DateTime.Today)
                yield return "Date of birth must be in the past";
        }

        private IEnumerable<string> ValidateHeight(int? height)
        {
            if (height.HasValue && (height < 100 || height > 250))
                yield return "Height must be between 100 and 250 cm";
        }

        private IEnumerable<string> ValidateWeight(int? weight)
        {
            if (weight.HasValue && (weight < 30 || weight > 200))
                yield return "Weight must be between 30 and 200 kg";
        }
    }
}
