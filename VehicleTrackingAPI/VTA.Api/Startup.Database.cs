using Couchbase.N1QL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VTA.Buckets.Buckets.VehicleBucket;
using VTA.Buckets.Models;
using VTA.Constants;

namespace VTA.Api
{
    public partial class Startup
    {
        public void InitializeDb(IApplicationBuilder app, IHostingEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var vehicleBucket = services.GetRequiredService<IVehicleBucketProvider>();
                var n1sql = $@"select v.*
                            from Vehicle v
                            where v.type = '{DocumentType.USER}' and v.username = 'admin'";
                var query = QueryRequest.Create(n1sql);
                query.ScanConsistency(ScanConsistency.RequestPlus);
                var result = vehicleBucket.GetBucket().Query<User>(query);
                if (result.Rows.Count == 0)
                {
                    var passwordHasher = services.GetRequiredService<IPasswordHasher<User>>();
                    var user = new User
                    {
                        Username = "admin",
                    };
                    user.Password = passwordHasher.HashPassword(user, "admin");

                    vehicleBucket.GetBucket().Insert(user.Username, new User
                    {
                        Username = user.Username,
                        Password = user.Password,
                    });
                }
            }
        }
    }
}
