using Wam.Core.Configuration;
using Wam.Core.ExtensionMethods;
using Wam.Core.Identity;
using Wam.Proxy;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

var azureCredential = CloudIdentity.GetCloudIdentity();
try
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        var appConfigurationUrl = builder.Configuration.GetRequiredValue("AzureAppConfiguration");
        Console.WriteLine($"Azure App Configuration URL: {appConfigurationUrl}");
        options.Connect(new Uri(appConfigurationUrl), azureCredential)
            .UseFeatureFlags();
    });
}
catch (Exception ex)
{
    throw new Exception("Failed to configure the Whack-A-Mole Proxy service, Azure App Configuration failed", ex);
}

builder.Services.AddHealthChecks();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddWamCoreConfiguration(builder.Configuration);

builder.Services
    .AddSingleton<IProxyConfigProvider, CustomProxyConfigProvider>()
    .AddReverseProxy();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();