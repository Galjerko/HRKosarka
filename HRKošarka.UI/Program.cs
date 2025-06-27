using Blazored.LocalStorage;
using Blazored.Toast.Services;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Components;
using HRKošarka.UI.Providers;
using HRKošarka.UI.Services;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection;
using MudBlazor.Services;

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
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
