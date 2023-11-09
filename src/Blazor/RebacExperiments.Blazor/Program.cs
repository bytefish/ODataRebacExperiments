
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using RebacExperiments.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Fast.Components.FluentUI;
using RebacExperiments.Blazor.Shared.Extensions;
using RebacExperiments.Blazor.Infrastructure;
using System.Text.Json;
using RebacExperiments.Shared.ApiSdk;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<CookieHandler>();

builder.Services.AddScoped<IAuthenticationProvider, AnonymousAuthenticationProvider>();

builder.Services
    .AddHttpClient<IRequestAdapter, HttpClientRequestAdapter>(client => client.BaseAddress = new Uri("https://localhost:5000"))
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddScoped<ApiClient>();

builder.Services.AddLocalization();

builder.Services.AddFluentUIComponents(options =>
{
    options.HostingModel = BlazorHostingModel.WebAssembly;
});

//When using icons and/or emoji replace the line above with the code below
//LibraryConfiguration config = new(ConfigurationGenerator.GetIconConfiguration(), ConfigurationGenerator.GetEmojiConfiguration());
//builder.Services.AddFluentUIComponents(config);

await builder.Build().RunAsync();
