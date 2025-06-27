using HRKošarka.UI.Services.Base;

namespace HRKošarka.UI.Contracts
{
    public interface IAuthenticationService
    {
        Task<AuthResponse> AuthenticateAsync(AuthRequest authRequest);
        Task<RegistrationResponse> RegisterAsync(RegistrationRequest registrationRequest);
        Task LogoutAsync();

    }
}
