using Blazored.LocalStorage;
using Blazored.Toast.Services;
using HRKošarka.UI.Components;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Providers;
using HRKošarka.UI.Services;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddMudServices();
builder.Services.AddHttpClient<IClient, Client>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7076");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddBlazoredLocalStorage();

// NO server-side auth - only Blazor client-side auth
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

//TODO: add static helpers for service registrations
builder.Services.AddScoped<IAgeCategoryService, AgeCategoryService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserPermissionCacheService, UserPermissionCacheService>();
builder.Services.AddScoped<IPermissionService, UserPermissionCacheService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
