// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApplicationBuilderExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.UserGroups.Api;

using DaprDemo.AspNetCore.BasePathFilter;
using DaprDemo.Dapr.Extension.Configuration;
using DaprDemo.Dapr.Extension.HealthChecks;
using DaprDemo.OpenTelemetry.Extensions;
using global::OpenTelemetry.Resources;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public static class WebApplicationBuilderExtensions
{
	/// <summary>
	/// Configures OpenTelemetry for logging, tracing and metrics using OTLP protocol. Source details come from
	/// environment variable <c>OTEL_RESOURCE_ATTRIBUTES</c>. Refer to OpenTelemetry documentation here for more
	/// details:
	/// https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/sdk-environment-variables.md
	/// and for service property here:
	/// https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/resource/semantic_conventions/README.md#service.
	/// </summary>
	/// <param name="builder">
	/// <see cref="WebApplicationBuilder"/> to configure OpenTelemetry logging, metrics and tracing for.
	/// </param>
	/// <returns>Updated <paramref name="builder"/>.</returns>
	public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		// Source Properties come from Environment Variables injected in Kubernetes
		var configureResource = ResourceBuilder
			.CreateDefault();

		builder.Services.AddOpenTelemetryServices(configureResource);
		builder.Logging.AddOpenTelemetryLogging(configureResource);

		return builder;
	}

	/// <summary>
	/// Adds health checks for the application including: "self"; and "dapr".
	/// </summary>
	/// <param name="builder">
	/// <see cref="WebApplicationBuilder"/> to configure Health Checks for.
	/// </param>
	/// <returns>Updated <paramref name="builder"/>.</returns>
	public static WebApplicationBuilder AddHealthChecks(this WebApplicationBuilder builder)
	{
		builder.Services.AddHealthChecks()
			.AddCheck("self", () => HealthCheckResult.Healthy())
			.AddDapr();

		return builder;
	}

	public static WebApplicationBuilder AddDapr(this WebApplicationBuilder builder)
	{
		builder.Services.AddDaprServices();
		builder.Configuration.AddDaprConfiguration(builder.Services);
		return builder;
	}

	public static WebApplicationBuilder AddBasePathMiddleware(this WebApplicationBuilder builder)
	{
		builder.Services.AddBasePathMiddleware(builder.Configuration);
		return builder;
	}
}