// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using DaprDemo.Shared.BasePathFilter;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string envVarPrefix = "DAPRDEMO_";
const string envVarVersion = "DOTNET_APP_VERSION";
var serviceName = Assembly.GetExecutingAssembly().GetName().Name!;
var serviceVersion = Environment.GetEnvironmentVariable(envVarVersion) // Passed in this way due to issue with `dotnet build`
	?? Assembly.GetExecutingAssembly().GetName().Version?.ToString();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(envVarPrefix);

builder.Services.AddBasePathMiddleware(builder.Configuration);

// Open Telemetry
var configureResource = ResourceBuilder
	.CreateDefault()
	.AddService(
		serviceName: serviceName,
		serviceVersion: serviceVersion,
		serviceInstanceId: Environment.MachineName);

builder.Services.AddOpenTelemetryTracing(options =>
{
	options.SetResourceBuilder(configureResource)
		.AddSource(serviceName)
		.SetSampler(new AlwaysOnSampler())
		.AddHttpClientInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc)
		.AddConsoleExporter();
});

builder.Logging.AddOpenTelemetry(options =>
{
	options
		.SetResourceBuilder(configureResource)
		.SetIncludeScopes(true)
		.SetIncludeFormattedMessage(true)
		.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc)
		.AddConsoleExporter();
});

builder.Services.AddOpenTelemetryMetrics(options =>
{
	options.SetResourceBuilder(configureResource)
		.AddRuntimeInstrumentation()
		.AddHttpClientInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc)
		.AddConsoleExporter();
});

builder.Services.AddHealthChecks();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var mySource = new ActivitySource(serviceName);

app.MapGet("/hello", () =>
{
	// Track work inside of the request
	using var activity = mySource.StartActivity();
	activity?.SetTag("foo", 1);
	activity?.SetTag("bar", "Hello, World!");
	activity?.SetTag("baz", new[] { 1, 2, 3 });

	return $"Hello from {serviceName}!";
});

app.MapGet("/version", () => new Dictionary<string, string?>
{
	["AssemblyInformationalVersion"] = Environment.GetEnvironmentVariable(envVarVersion),
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseForwardedHeaders();

// Enable Health Checks
app.UseHealthChecks("/health");

app.Run();