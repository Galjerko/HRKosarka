using HRKošarka.UI.Contracts;
using HRKošarka.UI.Models.Auth;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;

namespace HRKošarka.UI.Components.Pages
{
    public partial class Login 
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; } = default!;

        public LoginVM Model { get; set; } = new();
        public string Message { get; set; } = string.Empty;

        private bool _isLoading = false;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        protected override void OnInitialized()
        {
            Model = new LoginVM();
        }

        protected async Task HandleLogin()
        {
            if (!ValidateModel())
                return;

            _isLoading = true;
            Message = string.Empty;

            try
            {
                var authRequest = new AuthRequest
                {
                    EmailOrUsername = Model.Email,
                    Password = Model.Password
                };

                var authResponse = await AuthenticationService.AuthenticateAsync(authRequest);

                if (!string.IsNullOrEmpty(authResponse.Token))
                {
                    NavigationManager.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    Message = "Invalid credentials. Please try again.";
                }
            }
            catch (ApiException apiEx)
            {
                Message = ExtractErrorMessage(apiEx);
            }
            catch (Exception ex)
            {
                Message = $"Login failed. Please try again later. {ex}";
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        private string ExtractErrorMessage(ApiException apiEx)
        {
            try
            {
                if (!string.IsNullOrEmpty(apiEx.Response))
                {
                    using var document = JsonDocument.Parse(apiEx.Response);

                    if (document.RootElement.TryGetProperty("title", out var title))
                    {
                        return title.GetString() ?? "Login failed";
                    }

                    if (document.RootElement.TryGetProperty("message", out var message))
                    {
                        return message.GetString() ?? "Login failed";
                    }
                }
            }
            catch { }

            return apiEx.Response ?? "Login failed";
        }

        private bool ValidateModel()
        {
            if (string.IsNullOrWhiteSpace(Model.Email))
            {
                Message = "Please enter your email or username.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Model.Password))
            {
                Message = "Please enter your password.";
                return false;
            }

            return true;
        }

        private void TogglePasswordVisibility()
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
    }
}
