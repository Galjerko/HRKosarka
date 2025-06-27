using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HRKošarka.UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var isTokenPresent = await _localStorage.ContainKeyAsync("token");
            if (isTokenPresent is false )
            {
                return new AuthenticationState(user);
            }

            var savedToken = await _localStorage.GetItemAsync<string>("token");
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

            if(tokenContent.ValidTo < DateTime.UtcNow)
            {
                await _localStorage.RemoveItemAsync("token");
                return new AuthenticationState(user);
            }

            var claims = await GetClaims();
            user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            return new AuthenticationState(user);
        }

        public async Task LoggedIn()
        {
            var claims = await GetClaims();
            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(user);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }   

        public async Task LoggedOut()
        {
            await _localStorage.RemoveItemAsync("token");
            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = new AuthenticationState(nobody);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        private async Task<List<Claim>> GetClaims()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("token");
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }
    }
}
