
global using SchoolManagementAPI;
global using Microsoft.EntityFrameworkCore;

using System.Net;
using System.Data;
using Hangfire;
using SchoolManagementAPI.Interfaces;
using SchoolManagementAPI.Infrastructure.Repositories;
using SchoolManagementAPI.Infrastructure;
using System.Configuration;
using SchoolManagementAPI.Models.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication.OAuth;
using Quartz;
using NLog;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDatabase"));
    // Configure the context to use an in-memory store.
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
        // Register the OpenIddict core components.
        // Configure OpenIddict to use the Entity Framework Core stores and models.
        // Note: call ReplaceDefaultEntities() to replace the default entities.
        .AddCore(options =>
        {
            options
                .UseEntityFrameworkCore()
                .UseDbContext<ApplicationDbContext>();
        })

        // Register the OpenIddict server components.
        .AddServer(options =>
        {
            options.SetTokenEndpointUris("/token");
            options.AllowClientCredentialsFlow();
            options.AllowPasswordFlow();
            options.AcceptAnonymousClients();
            options.SetAccessTokenLifetime(TimeSpan.FromHours(6));
            options.AddDevelopmentEncryptionCertificate()
                   .AddDevelopmentSigningCertificate();
            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
            options
                .UseAspNetCore()
                .EnableTokenEndpointPassthrough()
                .DisableTransportSecurityRequirement();
        })
        .AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });


builder.Services.AddCors(
    options => options.AddPolicy("AllowCors",
    builder =>
    {
        builder.SetIsOriginAllowed(origin => true)
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
    })
);


builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.User.RequireUniqueEmail = false;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/\\";
    }
)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IISOptions>(options =>
{
    options.AutomaticAuthentication = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

//other classes that need the logger 
builder.Services.AddTransient<GenericHelper>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Auto Mapper Configurations
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IAdministrationRepository, AdministrationRepository>();

builder.Services.AddLogging(c => c.ClearProviders());

var app = builder.Build();

Console.Write("App init..");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = new JsonExceptionMiddleware().Invoke
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(options =>
{
    options.MapControllers();
    options.MapDefaultControllerRoute();
});

app.Run();

