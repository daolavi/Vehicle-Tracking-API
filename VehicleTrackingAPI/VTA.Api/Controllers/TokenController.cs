using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VTA.Models.Request;
using VTA.Services.AuthenticationService;

namespace VTA.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IAuthenticationService authenticationService;

        public TokenController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]LoginRequest loginRequest)
        {
            IActionResult response = Unauthorized();
            var user = authenticationService.Authenticate(loginRequest);
            if (user != null)
            {
                var tokenString = authenticationService.BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }
    }
}
