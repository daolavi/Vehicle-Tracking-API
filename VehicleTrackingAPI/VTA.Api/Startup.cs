using Couchbase.Extensions.Caching;
using Couchbase.Extensions.DependencyInjection;
using Geocoding;
using Geocoding.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;
using VTA.Api.Validation;
using VTA.Buckets.Buckets.VehicleBucket;
using VTA.Buckets.Models;
using VTA.Services.AuthenticationService;
using VTA.Services.LocationNameService;
using VTA.Services.VehicleService;

namespace VTA.Api
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add Jwt Authen
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            });

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "VTA API", Version = "v1" });
            });

            // Support for CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                .Build());
            });

            services.AddCouchbase(Configuration.GetSection("Couchbase"))
                    // Vehicle bucket for persist data including : register vehicle, record postion
                    .AddCouchbaseBucket<IVehicleBucketProvider>(Configuration["VehicleBucketName"], Configuration["VehicleBucketPassword"])
                    // Location bucket is an ephemeral bucket for caching address in-memory. 
                    // When converting from latitude, longtitude to address, the service will lookup cache first, if not existed, it will call to Google Map Api
                    .AddDistributedCouchbaseCache(Configuration["LocationBucketName"], Configuration["LocationBucketPassword"], opt => { });

            // AuthenticationService is used for authenticate admin account and issuing token
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // VehicleService is used for register vehicle, record position, get current postion and get positions during certain time
            services.AddScoped<IVehicleService, VehicleService>();

            // PasswordHasher is used for hasing password
            services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

            // LocationNameService is used for converting latitude, longtitude to address
            services.AddTransient<ILocationNameService, LocationNameService>();

            // GoogleGeocoder is used for making request to Google Map Api
            services.AddTransient<IGeocoder, GoogleGeocoder>(geoCoder => new GoogleGeocoder(Configuration["APIKey"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            // Release CouchbaseService once application stopped
            applicationLifetime.ApplicationStopped.Register(() =>
            {
                app.ApplicationServices.GetRequiredService<ICouchbaseLifetimeService>().Close();
            });

            app.UseMvc();

            // Create dummy data for Admin
            InitializeDb(app, env);
        }
    }
}
