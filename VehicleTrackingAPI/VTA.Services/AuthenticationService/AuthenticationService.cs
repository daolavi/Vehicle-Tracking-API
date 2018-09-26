using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VTA.Buckets.Models;
using VTA.Buckets.Repositories;
using VTA.Constants;
using VTA.Models.Request;

namespace VTA.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration config;

        private readonly IPasswordHasher<User> passwordHasher;

        private readonly IVehicleRepository vehicleRepository;

        public AuthenticationService(IVehicleRepository vehicleRepository, IConfiguration config, IPasswordHasher<User> passwordHasher)
        {
            this.vehicleRepository = vehicleRepository;
            this.config = config;
            this.passwordHasher = passwordHasher;
        }

        public User Authenticate(LoginRequest loginRequest)
        {
            var user = vehicleRepository.GetUser(loginRequest.Username);
            if (user != null)
            {
                var verifyResult = passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
                if (verifyResult == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }
            return null;
        }

        public string BuildToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimNames.Username, user.Username),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["Jwt:Issuer"],
                                            config["Jwt:Issuer"],
                                            claims,
                                            expires: DateTime.Now.AddMinutes(30),
                                            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
