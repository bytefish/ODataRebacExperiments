
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using RebacExperiments.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using Microsoft.OData.Client;
using RebacExperiments.Blazor.Shared.Infrastructure;
using RebacExperiments.Blazor.Shared.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 
builder.Services.AddSingleton<ODataResponseParser>((sp) =>
{
    var logger = sp.GetRequiredService<ILogger<ODataResponseParser>>();

    return new ODataResponseParser(logger, new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });
});

builder.Services.AddTransient<CookieHandler>();

builder.Services
    .AddHttpClient("Task Management Service")
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddLocalization();

builder.Services.AddFluentUIComponents(options =>
{
    options.HostingModel = BlazorHostingModel.WebAssembly;
});

//When using icons and/or emoji replace the line above with the code below
//LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
//builder.Services.AddFluentUIComponents(config);

await builder.Build().RunAsync();
