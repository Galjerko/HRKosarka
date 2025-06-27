using HRKošarka.UI.Contracts;
using HRKošarka.UI.Models.Auth;
using HRKošarka.UI.Services.Base; // For RegistrationRequest
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;

namespace HRKošarka.UI.Components.Pages
{
    public partial class Register
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; } = default!;

        public RegisterVM Model { get; set; } = new();
        public string Message { get; set; } = string.Empty;

        private bool _isLoading = false;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        protected override void OnInitialized()
        {
            Model = new RegisterVM();
        }

        protected async Task HandleRegister()
        {

            _isLoading = true;
            Message = string.Empty;

            try
            {
                var registrationRequest = new RegistrationRequest
                {
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    Email = Model.Email,
                    UserName = Model.UserName,
                    Password = Model.Password
                };

                var registrationResponse = await AuthenticationService.RegisterAsync(registrationRequest);

                if (!string.IsNullOrEmpty(registrationResponse.UserId))
                {
                    // auto login bi bilo super
                    NavigationManager.NavigateTo("/login?message=Registration successful. Please login with your credentials.");
                }
                else
                {
                    Message = "Registration failed. Please try again.";
                }
            }
            catch (ApiException apiEx)
            {
                Message = ExtractErrorMessage(apiEx);
            }
            catch (Exception ex)
            {
                Message = $"Registration failed. Please try again later. {ex}";
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        void TogglePasswordVisibility()
        {
            if (_passwordInput == InputType.Password)
            {
                _passwordInput = InputType.Text;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
            }
            else
            {
                _passwordInput = InputType.Password;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            }
        }
        private string ExtractErrorMessage(ApiException apiEx)
        {
            try
            {
                if (!string.IsNullOrEmpty(apiEx.Response))
                {
                    using var document = JsonDocument.Parse(apiEx.Response);

                    if (document.RootElement.TryGetProperty("errors", out var errorsElement) &&
                        errorsElement.ValueKind == JsonValueKind.Object)
                    {
                        var errorMessages = new List<string>();

                        foreach (var property in errorsElement.EnumerateObject())
                        {
                            if (property.Value.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var error in property.Value.EnumerateArray())
                                {
                                    var errorText = error.GetString();
                                    if (!string.IsNullOrEmpty(errorText))
                                    {
                                        errorMessages.Add(errorText);
                                    }
                                }
                            }
                        }

                        if (errorMessages.Any())
                        {
                            return string.Join(". ", errorMessages);
                        }
                    }

                    if (document.RootElement.TryGetProperty("title", out var title))
                    {
                        return title.GetString() ?? "Registration failed";
                    }
                    if (document.RootElement.TryGetProperty("message", out var message))
                    {
                        return message.GetString() ?? "Registration failed";
                    }
                }
            }
            catch { }

            return apiEx.Response ?? "Registration failed";
        }


    }
}
