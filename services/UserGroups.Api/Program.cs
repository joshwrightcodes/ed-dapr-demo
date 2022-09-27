// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using BasePathFilter;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "EdDemo.Dapr.UserGroupsApi";
const string assemblyVersion = "1.0.0";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBasePathMiddleware(builder.Configuration);

// Open Telemetry
var configureResource = ResourceBuilder
	.CreateDefault()
	.AddService(
		serviceName: serviceName,
		serviceVersion: assemblyVersion,
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
	using var activity = mySource.StartActivity("SayHello");
	activity?.SetTag("foo", 1);
	activity?.SetTag("bar", "Hello, World!");
	activity?.SetTag("baz", new int[] { 1, 2, 3 });

	return "Hello, World!";
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