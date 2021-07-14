using Application.Models.Authentication;
using System.Threading.Tasks;

namespace Application.Contracts.Identity
{
    /// <summary>
    /// Used in "Identity project"
    /// </summary>
    public interface IAuthenticationService
    {
        Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
        Task<RegistrationResponse> RegisterAsync(RegistrationRequest request);
    }
}
