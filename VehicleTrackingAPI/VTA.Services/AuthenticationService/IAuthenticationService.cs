using VTA.Buckets.Models;
using VTA.Models.Request;

namespace VTA.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        User Authenticate(LoginRequest loginRequest);

        string BuildToken(User user);
    }
}
