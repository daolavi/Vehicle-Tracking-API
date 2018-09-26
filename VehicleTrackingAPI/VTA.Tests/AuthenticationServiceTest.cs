using Couchbase.N1QL;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using VTA.Buckets.Models;
using VTA.Buckets.Repositories;
using VTA.Models.Request;
using VTA.Services.AuthenticationService;
using Xunit;

namespace VTA.Tests
{
    public class AuthenticationServiceTest
    {
        private IPasswordHasher<User> passwordHasher;

        private readonly IVehicleRepository vehicleRepository;

        private readonly IConfiguration config;

        public AuthenticationServiceTest()
        {
            passwordHasher = new PasswordHasher<User>();
            vehicleRepository = A.Fake<IVehicleRepository>();
            config = A.Fake<IConfiguration>();
        }

        [Fact]
        public void Authenticate_Success()
        {
            var user = new User { Username = "user1" };
            var password = "correctpassword";
            var hashedPassword = passwordHasher.HashPassword(user, "correctpassword");

            var authenticateService = new AuthenticationService(vehicleRepository, config, passwordHasher);
            var loginRequest = new LoginRequest
            {
                Username = user.Username,
                Password = password,
            };

            A.CallTo(() => vehicleRepository.GetUser(A<string>._)).Returns(new User { Username = user.Username, Password = hashedPassword });

            var model = authenticateService.Authenticate(loginRequest);
            Assert.NotNull(model);
        }

        [Fact]
        public void Authenticate_Fail()
        {
            var user = new User { Username = "user1" };
            var password = "wrongpassword";
            var hashedPassword = passwordHasher.HashPassword(user, "correctpassword");

            var authenticateService = new AuthenticationService(vehicleRepository, config, passwordHasher);
            var loginRequest = new LoginRequest
            {
                Username = user.Username,
                Password = password,
            };

            A.CallTo(() => vehicleRepository.GetUser(A<string>._)).Returns(new User { Username = user.Username, Password = hashedPassword });

            var model = authenticateService.Authenticate(loginRequest);
            Assert.Null(model);
        }
    }
}
