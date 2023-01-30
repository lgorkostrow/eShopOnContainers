using Autofac;
using Autofac.Extensions.DependencyInjection;
using Coupon.API.Extensions;
using Coupon.Application;
using Coupon.Infrastructure;
using Coupon.Infrastructure.Mongo;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwagger()
    .AddApiVersioning()
    .AddAppInsight(configuration)
    .AddCustomMVC()
    .AddCustomOptions(configuration)
    .AddCustomIntegrations(configuration)
    .AddEventBus(configuration)
    .AddCustomHealthCheck(configuration);

// Infrastructure layer
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices();

// Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

var app = builder.Build();

app.SeedDatabaseStrategy<CouponContext>(context => new CouponSeed().SeedAsync(context).Wait());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

var pathBase = configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
    endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });
});

app.RegisterApplicationIntegrationEvents();

app.Run();