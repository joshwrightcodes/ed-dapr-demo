// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using Dapr.Client;
using Dapr.Extensions.Configuration;
using DaprDemo.Shared.BasePathFilter;
using DaprDemo.Users.Api;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string envVarPrefix = "APP_";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(envVarPrefix);

builder.Services.AddBasePathMiddleware(builder.Configuration);
builder.AddOpenTelemetry();
builder.AddDapr();
builder.AddHealthChecks();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration); // Bad, for demo purposes only
var app = builder.Build();

app.MapGet("/hello", ([FromServices] ILogger<Program> logger) =>
{
	// Track work inside of the request
	logger.LogInformation("Saying Hello");
	return $"Hello from {Assembly.GetExecutingAssembly().GetName().Name!}!";
});

app.MapGet("/version", ([FromServices] IConfigurationRoot config)
	=> new Dictionary<string, string?>
	{
		["AssemblyInformationalVersion"] = config.GetValue<string>("VERSION"),
	});

app.MapGet("/config", ([FromServices] IConfigurationRoot configurationRoot)
	=> configurationRoot.GetDebugView());

app.MapGet(
	"mail/send",
	(
		[FromQuery] string email,
		[FromQuery] string name,
		[FromServices] DaprClient daprClient,
		[FromServices] ILogger<Program> logger,
		CancellationToken cancellationToken) =>
	{
		const string sendMailBinding = "sendmail";

		logger.LogInformation("Sending email to {EmailAddress}", email);
		return daprClient.InvokeBindingAsync(
			"dapr-demo-users-api-sendmail",
			"create",
			$"<html><body><p>Hello <b>{name}</b>!</p></body><html>",
			new Dictionary<string, string>
			{
				["emailFrom"] = "sample@wright.codes",
				["emailTo"] = email,
				["subject"] = $"Hello <b>{name}</b>!",
			},
			cancellationToken);
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