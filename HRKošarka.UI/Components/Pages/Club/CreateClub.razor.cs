using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Text.RegularExpressions;

namespace HRKošarka.UI.Components.Pages.Club
{
    public partial class CreateClub
    {
        [Inject] private IClubService ClubService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;

        private CreateClubCommand _model = new();
        private bool _isLoading = false;
        private int? _foundedYear = DateTime.Now.Year;
        private MudForm _form = default!;
        private bool _isFormValid = false;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (!authState.User.Identity?.IsAuthenticated ?? false)
            {
                NavigationManager.NavigateTo("/login");
                return;
            }

            if (!authState.User.IsInRole("Administrator"))
            {
                Snackbar.Add("You need Administrator privileges to create clubs.", Severity.Warning);
                NavigationManager.NavigateTo("/clubs");
                return;
            }

            _foundedYear = DateTime.Now.Year;
        }

        private async Task HandleSubmit()
        {
            await _form.Validate();

            if (!_form.IsValid)
            {
                Snackbar.Add("Please fix the validation errors before submitting.", Severity.Warning);
                return;
            }

            _isLoading = true;

            try
            {
                if (_foundedYear.HasValue)
                {
                    _model.FoundedYear = new DateTime(_foundedYear.Value, 1, 1);
                }

                var response = await ClubService.CreateClub(_model);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Club created successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/clubs");
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error + "!", Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to create club", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred. Please try again.", Severity.Error);
                Console.WriteLine($"Error creating club: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
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
