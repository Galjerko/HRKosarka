using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace HRKošarka.UI.Components.Pages.Club
{
    public partial class EditClub : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private IClubService ClubService { get; set; } = default!;

        private UpdateClubCommand _model = new();
        private ClubDetailsDTO? _club;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private int? _foundedYear;
        private MudForm _form = default!;
        private bool _isFormValid = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await SetClubPermissions(Id);

            await LoadClubData();
        }

        private async Task LoadClubData()
        {
            _isLoading = true;

            try
            {
                var response = await ClubService.GetClubDetails(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _club = response.Data;

                    if (!_club.IsActive)
                    {
                        Snackbar.Add("Cannot edit inactive club.", Severity.Warning);
                        NavigationManager.NavigateTo($"/clubs/{Id}");
                        return;
                    }

                    _foundedYear = _club.FoundedYear;

                    _model = new UpdateClubCommand
                    {
                        Id = _club.Id,
                        Name = _club.Name ?? string.Empty,
                        City = _club.City ?? string.Empty,
                        Description = _club.Description ?? string.Empty,
                        Email = _club.Email ?? string.Empty,
                        PhoneNumber = _club.PhoneNumber ?? string.Empty,
                        Website = _club.Website ?? string.Empty,
                        Address = _club.Address ?? string.Empty,
                        PostalCode = _club.PostalCode ?? string.Empty,
                        VenueName = _club.VenueName ?? string.Empty,
                        VenueCapacity = _club.VenueCapacity,
                        FoundedYear = new DateTimeOffset(new DateTime(_club.FoundedYear, 1, 1))
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
                        Snackbar.Add(response.Message ?? "Failed to load club details", Severity.Error);
                    }

                    NavigationManager.NavigateTo("/clubs");
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading club details.", Severity.Error);
                Console.WriteLine($"Error loading club details: {ex.Message}");

                NavigationManager.NavigateTo("/clubs");
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

            // Check permission before submitting
            if (!CurrentPermissions.CanEdit)
            {
                Snackbar.Add("You don't have permission to edit this club", Severity.Warning);
                return;
            }

            _isProcessing = true;

            try
            {
                if (_foundedYear.HasValue)
                {
                    _model.FoundedYear = new DateTime(_foundedYear.Value, 1, 1);
                }

                var response = await ClubService.UpdateClub(Id, _model);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Club updated successfully!", Severity.Success);
                    NavigationManager.NavigateTo($"/clubs/{Id}");
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
                        Snackbar.Add(response.Message ?? "Failed to update club", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred. Please try again.", Severity.Error);
                Console.WriteLine($"Error updating club: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task OnImageChanged((string? name, string? contentType, byte[]? bytes) image)
        {
            _model.ImageName = image.name;
            _model.ImageContentType = image.contentType;
            _model.ImageBytes = image.bytes;
        }

        private IEnumerable<string> ValidateClubName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                yield return "Club name is required";
            }
            else if (name.Length > 150)
            {
                yield return "Club name must not exceed 150 characters";
            }
        }

        private IEnumerable<string> ValidateFoundedYearNumeric(int? year)
        {
            if (year == null)
            {
                yield return "Founded year is required";
            }
            else if (year < 1800)
            {
                yield return "Founded year cannot be before 1800";
            }
            else if (year > DateTime.Now.Year)
            {
                yield return "Founded year cannot be in the future";
            }
        }

        private IEnumerable<string> ValidateEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                if (email.Length > 255)
                {
                    yield return "Email must not exceed 255 characters";
                }
                else if (!IsValidEmail(email))
                {
                    yield return "Invalid email format";
                }
            }
        }

        private IEnumerable<string> ValidatePhoneNumber(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                if (phone.Length > 20)
                {
                    yield return "Phone number must not exceed 20 characters";
                }
                else if (!IsValidPhoneNumber(phone))
                {
                    yield return "Invalid phone number format";
                }
            }
        }

        private IEnumerable<string> ValidateWebsite(string website)
        {
            if (!string.IsNullOrEmpty(website))
            {
                if (website.Length > 255)
                {
                    yield return "Website URL must not exceed 255 characters";
                }
                else if (!IsValidUrl(website))
                {
                    yield return "Invalid website URL format";
                }
            }
        }

        private IEnumerable<string> ValidateUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Length > 500)
                {
                    yield return "URL must not exceed 500 characters";
                }
                else if (!IsValidUrl(url))
                {
                    yield return "Invalid URL format";
                }
            }
        }

        private IEnumerable<string> ValidatePostalCode(string postalCode)
        {
            if (!string.IsNullOrEmpty(postalCode))
            {
                if (postalCode.Length > 20)
                {
                    yield return "Postal code must not exceed 20 characters";
                }
                else if (!IsValidCroatianPostalCode(postalCode))
                {
                    yield return "Invalid postal code format";
                }
            }
        }

        private IEnumerable<string> ValidateVenueName(string venueName)
        {
            if (!string.IsNullOrEmpty(venueName))
            {
                if (venueName.Length > 200)
                {
                    yield return "Venue name must not exceed 200 characters";
                }
            }
        }

        private IEnumerable<string> ValidateVenueCapacityNumeric(int? capacity)
        {
            if (capacity.HasValue)
            {
                if (capacity <= 0)
                {
                    yield return "Venue capacity must be a positive number";
                }
                else if (capacity > 200000)
                {
                    yield return "Venue capacity cannot exceed 200,000";
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            var patterns = new[]
            {
                @"^\+385\d{8,9}$",
                @"^0\d{8,9}$",
                @"^\d{8,9}$"
            };

            return patterns.Any(pattern => Regex.IsMatch(phone, pattern));
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        private bool IsValidCroatianPostalCode(string postalCode)
        {
            return Regex.IsMatch(postalCode, @"^\d{5}$") &&
                   int.TryParse(postalCode, out int code) &&
                   code >= 10000 && code <= 53296;
        }
    }
}
