
using System.Text;
using KC.LMS.Server.Services;
using KC.LMS.Service;
using KC.LMS.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace KC.LMS.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

            var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("Missing 'Jwt' configuration section.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                        ValidateLifetime = true,
                    };
                });
            builder.Services.AddAuthorization(options =>
            {
                // Sample scoped policy: user must be OrgManager of the org in route value "organizationId",
                // hold OrgManager tenant-wide, or be TenantAdmin.
                options.AddPolicy("OrgManagerOfRoute", policy =>
                    policy.AddRequirements(new Authorization.ScopeRequirement(
                        KC.LMS.Storage.Entities.RoleNames.OrgManager,
                        KC.LMS.Storage.Entities.AccessScopeType.Organization,
                        "organizationId")));
            });
            builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, Authorization.ScopeAuthorizationHandler>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.MapStaticAssets();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
