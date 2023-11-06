
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using RebacExperiments.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using RebacExperiments.Blazor.Shared.Extensions;
using RebacExperiments.Blazor.Infrastructure;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<JsonSerializerOptions>(sp =>
{
    return new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
});

builder.Services.AddScoped<CookieHandler>();

builder.Services
    .AddHttpClient<ODataService>()
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped<ODataResponseParser>();
builder.Services.AddScoped<ODataService>();

builder.Services.AddLocalization();

builder.Services.AddFluentUIComponents(options =>
{
    options.HostingModel = BlazorHostingModel.WebAssembly;
});

//When using icons and/or emoji replace the line above with the code below
//LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
//builder.Services.AddFluentUIComponents(config);

await builder.Build().RunAsync();
