using Wam.Core.ExtensionMethods;
using Wam.Core.Identity;
using Wam.Proxy;
using Wam.Proxy.ExtensionMethods;
using Yarp.ReverseProxy.Configuration;

var corsPolicyName = "DefaultCors";
var builder = WebApplication.CreateBuilder(args);

var azureCredential = CloudIdentity.GetCloudIdentity();
try
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        var appConfigurationUrl = builder.Configuration.GetRequiredValue("AzureAppConfiguration");
        options.Connect(new Uri(appConfigurationUrl), azureCredential)
            .UseFeatureFlags();
    });
}
catch (Exception ex)
{
    throw new Exception("Failed to configure the Whack-A-Mole Realtime service, Azure App Configuration failed", ex);
}

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddServiceConfiguration(builder.Configuration);
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

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapReverseProxy();
app.Run();