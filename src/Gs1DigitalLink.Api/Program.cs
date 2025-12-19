using Gs1DigitalLink.Api.Formatters;
using Gs1DigitalLink.Api.Services;
using Gs1DigitalLink.Core;
using Gs1DigitalLink.Core.Resolution;
using Gs1DigitalLink.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDigitalLinkCore();
builder.Services.AddDigitalLinkInfrastructure();
builder.Services.AddScoped<ILanguageContext, HttpLanguageContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization(options => options.ResourcesPath = "Formatters/Views/Resources");
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    options.ViewLocationFormats.Add("/Formatters/Views/Shared/{1}/{0}.cshtml");
});
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.AddSupportedUICultures("en", "fr", "nl", "de");
    options.SetDefaultCulture("en");
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddControllersWithViews(options =>
{
    options.OutputFormatters.Insert(0, new HtmlViewFormatter());
    options.RespectBrowserAcceptHeader = true;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapStaticAssets().ShortCircuit();
app.UseRequestLocalization();
app.UseExceptionHandler("/error");
app.MapControllers();
app.Run();
