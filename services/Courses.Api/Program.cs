// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using DaprDemo.Courses.Api;

const string envVarPrefix = "APP_";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(envVarPrefix);
builder.Configuration.AddJsonFile("appsettings.k8s.json", optional: true, reloadOnChange: true);

builder.AddBasePathMiddleware();
builder.AddOpenTelemetry();
builder.AddDapr();
builder.AddHealthChecks();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.AddApiVersioning();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration); // Bad, for demo purposes only
WebApplication app = builder.Build();

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