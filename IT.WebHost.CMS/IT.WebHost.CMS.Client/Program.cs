using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorBlueprint.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorBlueprintComponents();

await builder.Build().RunAsync();
