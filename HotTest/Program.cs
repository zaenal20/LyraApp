using Blazored.LocalStorage;
using Fluxor;
using Lyra.Core.API;
using Lyra.Data.API;
using Lyra.Data.Crypto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using UserLibrary.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

Signatures.Switch(true);

builder.Services.AddBlazoredLocalStorage();
var networkid = builder.Configuration["network"];
builder.Services.AddTransient<ILyraAPI>(a => LyraRestClient.Create(networkid, Environment.OSVersion.ToString(), "Nebula", "1.0"/*, $"http://nebula.{networkid}.lyra.live:{Neo.Settings.Default.P2P.WebAPI}/api/Node/"*/));

var currentAssembly = typeof(Program).Assembly;
var libAssembly = typeof(UserLibrary.Data.WalletView).Assembly;
builder.Services.AddFluxor(options => options.ScanAssemblies(libAssembly, currentAssembly));

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddTransient<NebulaConsts>();
builder.Services.AddSingleton<PeriodicExecutor>(x =>
    new PeriodicExecutor(networkid)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
