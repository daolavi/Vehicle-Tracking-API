using Couchbase.N1QL;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using VTA.Buckets.Models;
using VTA.Models.Request;
using VTA.Services.AuthenticationService;
using Xunit;

namespace VTA.Tests
{
    public class AuthenticationServiceTest
    {
        //private IPasswordHasher<User> passwordHasher;

        //private readonly IVehicleBucketProvider vehicleBucket;

        //private readonly IConfiguration config;

        //public AuthenticationServiceTest()
        //{
        //    passwordHasher = new PasswordHasher<User>();
        //    vehicleBucket = A.Fake<IVehicleBucketProvider>();
        //    config = A.Fake<IConfiguration>();
        //}

        //[Fact]
        //public void Authenticate_Success()
        //{
        //    var user = new User { Username = "user1" };
        //    var hashedPassword = passwordHasher.HashPassword(user, "test");

        //    var authenticateService = new AuthenticationService(vehicleBucket, config, passwordHasher);
        //    var loginRequest = new LoginRequest
        //    {
        //        Username = user.Username,
        //        Password = hashedPassword,
        //    };
        //    var queryResult = new QueryResult<User>();
        //    queryResult = queryResult.Concat(new List<User> { new User { } });
        //    A.CallTo(() => vehicleBucket.GetBucket().Query<User>(A<IQueryRequest>._)).Returns(queryResult);

        //    var model = authenticateService.Authenticate(loginRequest);
        //    Assert.NotNull(model);
        //}
    }
}
