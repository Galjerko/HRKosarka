using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Providers;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HRKošarka.UI.Services
{
    public class AuthenticationService : BaseHttpService, IAuthenticationService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly NavigationManager _navigationManager;
        private readonly IUserPermissionCacheService _permissionCacheService;

        public AuthenticationService(IClient client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider, NavigationManager navigationManager, IUserPermissionCacheService permissionCacheService) : base(client, localStorage)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _navigationManager = navigationManager;
            _permissionCacheService = permissionCacheService;
        }

        public async Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest)
        {
            var response = await _client.LoginAsync(authRequest);

            if (!string.IsNullOrEmpty(response.Token))
            {
                await _localStorage.SetItemAsync("token", response.Token);
                await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();

                _permissionCacheService.ClearCache();
            }

            return response;
        }

        public async Task LogoutAsync()
        {
            _permissionCacheService.ClearCache();
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
            _navigationManager.NavigateTo("/", forceLoad: true);
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationRequest)
        {
            var response = await _client.RegisterAsync(registrationRequest);
            return response;
        }
    }
}
