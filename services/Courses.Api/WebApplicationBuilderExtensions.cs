// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApplicationBuilderExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.UserGroups.Api;

using DaprDemo.Shared.HealthChecks.DaprHealth;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		var configureResource = ResourceBuilder
			.CreateDefault();

		builder.Services.AddOpenTelemetryTracing(options =>
		{
			options.SetResourceBuilder(configureResource)
				.AddSource()
				.SetSampler(new AlwaysOnSampler())
				.AddHttpClientInstrumentation()
				.AddAspNetCoreInstrumentation()
				.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc);
		});

		builder.Logging.AddOpenTelemetry(options =>
		{
			options
				.SetResourceBuilder(configureResource)
				.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc);

			options.IncludeScopes = true;
			options.IncludeFormattedMessage = true;
		});

		builder.Services.AddOpenTelemetryMetrics(options =>
		{
			options.SetResourceBuilder(configureResource)
				.AddRuntimeInstrumentation()
				.AddHttpClientInstrumentation()
				.AddAspNetCoreInstrumentation()
				.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc);
		});

		return builder;
	}

	public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
	{
		builder.Services.AddHealthChecks()
			.AddCheck("self", () => HealthCheckResult.Healthy())
			.AddDapr();

		return builder;
	}
}