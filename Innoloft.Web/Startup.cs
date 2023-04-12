using System.Text;
using Innoloft.Cache.Redis;
using Innoloft.Cache.Redis.Implementation;
using Innoloft.Domain.Net;
using Innoloft.Domain.Net.Implementation;
using Innoloft.Domain.Repositories;
using Innoloft.Domain.Users;
using Innoloft.EntityFramework;
using Innoloft.EntityFramework.Repositories;
using Innoloft.Web.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Innoloft.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options => { options.Filters.Add(typeof(StructuredResponseActionFilter)); });
        
        // Add DbContext and configure connection string
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection"),
                new MariaDbServerVersion(new Version(10, 6, 5))));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetSection("Redis:Configuration").Value;
            options.InstanceName = Configuration.GetSection("Redis:InstanceName").Value;
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
        
        services.AddHttpClient();

        services.AddScoped<ICacheRepository, CacheRepository>();
        services.AddScoped<IJsonPlaceholderClient, JsonPlaceholderClient>();
        
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IEventParticipantRepository, EventParticipantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Add Swagger
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"}); });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        // Use Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}