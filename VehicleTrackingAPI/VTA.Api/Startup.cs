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
                    .AddCouchbaseBucket<IVehicleBucketProvider>(Configuration["VehicleBucketName"], Configuration["VehicleBucketPassword"])
                    .AddDistributedCouchbaseCache(Configuration["LocationBucketName"], Configuration["LocationBucketPassword"], opt => { });

            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IVehicleService, VehicleService>();

            services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddTransient<ILocationNameService, LocationNameService>();
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
