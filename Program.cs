
global using SchoolManagementAPI;
global using SchoolManagementAPI.Interfaces;
global using SchoolManagementAPI.Infrastructure.Repositories;
global using SchoolManagementAPI.Infrastructure;
global using SchoolManagementAPI.Models.User;
global using SchoolManagementAPI.ViewModels.Application;
global using SchoolManagementAPI.ViewModels.Role;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Diagnostics;
global using System;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Net;
global using System.Security.Authentication;
global using Newtonsoft.Json;


using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiDatabase"));
    // Configure the context to use an in-memory store.
    options.UseOpenIddict();
});

// Auto Mapper Configurations
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.User.RequireUniqueEmail = false;
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/\\";
    }
)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

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
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IAdministrationRepository, AdministrationRepository>();

var app = builder.Build();

Console.WriteLine("App init..");

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    ExceptionHandler = new JsonExceptionMiddleware().Invoke
});

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        Console.WriteLine($"You have {pendingMigrations.Count()} pending migrations to apply.");
        Console.WriteLine("Applying pending migrations now");
        await context.Database.MigrateAsync();
    }

    var lastAppliedMigration = (await context.Database.GetAppliedMigrationsAsync()).Last();

    Console.WriteLine($"You're on schema version: {lastAppliedMigration}");
}

app.Run();
