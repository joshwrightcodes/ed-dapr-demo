// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Reflection;
using Dapr.Client;
using DaprDemo.Shared.BasePathFilter;
using DaprDemo.UserGroups.Api;
using Microsoft.AspNetCore.Mvc;

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

app.MapGet("/version", ([FromServices] IConfiguration configuration)
	=> new Dictionary<string, string?>
	{
		["AssemblyInformationalVersion"] = configuration.GetValue<string>("APP_VERSION"),
	});

app.MapGet("/config", ([FromServices] IConfiguration configuration)
	=> configuration.AsEnumerable());

app.MapGet(
	"mail/send",
	(
		[FromQuery] string email,
		[FromQuery] string name,
		[FromServices] IConfiguration configuration,
		[FromServices] DaprClient daprClient,
		[FromServices] ILogger<Program> logger,
		CancellationToken cancellationToken) =>
	{
		const string sendMailBinding = "sendmail";

		logger.LogInformation("Sending email to {EmailAddress}", email);
		return daprClient.InvokeBindingAsync(
			"dapr-demo-users-api-sendmail",
			"create",
			$"<html><body><p>Hello <b>{name}</b>!</p><p>Email sent from service {Assembly.GetExecutingAssembly().GetName().Name!} ({configuration.GetValue<string>("APP_VERSION")}) on host {Dns.GetHostName()}.</p></body><html>",
			new Dictionary<string, string>
			{
				["emailFrom"] = $"{Dns.GetHostName()}@daprdemo.wright.codes",
				["emailTo"] = email,
				["subject"] = $"Hello {name}!",
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