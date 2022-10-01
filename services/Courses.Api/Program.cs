// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using DaprDemo.Shared.BasePathFilter;
using DaprDemo.UserGroups.Api;
using Microsoft.AspNetCore.Mvc;

const string envVarPrefix = "DAPRDEMO_";
const string envVarVersion = "DOTNET_APP_VERSION";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
	.AddEnvironmentVariables(envVarPrefix);

builder.Services.AddBasePathMiddleware(builder.Configuration);
builder.AddOpenTelemetry();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/hello", ([FromServices] ILogger<Program> logger) =>
{
	// Track work inside of the request
	logger.LogInformation("Saying Hello");
	return $"Hello from {Assembly.GetExecutingAssembly().GetName().Name!}!";
});

app.MapGet("/version", ()
	=> new Dictionary<string, string?>
	{
		["AssemblyInformationalVersion"] = Environment.GetEnvironmentVariable(envVarVersion),
	});

app.MapGet("/config", ([FromServices] IConfigurationRoot configurationRoot)
	=> configurationRoot.GetDebugView());

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