using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Providers;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace HRKošarka.UI.Services
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationService(IClient client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider) : base(client, localStorage)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest)
        {
            var response = await _client.LoginAsync(authRequest);

            if (!string.IsNullOrEmpty(response.Token))
            {
                await _localStorage.SetItemAsync("token", response.Token);
                await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();
            }

            return response;
        }

        public async Task LogoutAsync()
        {
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationRequest)
        {
            var response = await _client.RegisterAsync(registrationRequest);
            return response;
        }
    }
}
