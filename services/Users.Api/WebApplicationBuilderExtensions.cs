// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApplicationBuilderExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;

namespace DaprDemo.Users.Api;

using Dapr.Client;
using Dapr.Extensions.Configuration;
using DaprDemo.Shared.HealthChecks.DaprHealth;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
		builder.Services.AddDaprClient(configure =>
		{
			configure.UseGrpcChannelOptions(new GrpcChannelOptions
			{
				HttpHandler = new 
			})
		});

		var options = builder.Configuration.GetSection(nameof(DaprOptions)).Get<DaprOptions>();

		if (!string.IsNullOrWhiteSpace(options.SecretStore))
		{
			builder.Configuration.AddDaprSecretStore(
				options.SecretStore,
				options.SecretDescriptors?.Select(sd => new DaprSecretDescriptor(sd)) ?? new List<DaprSecretDescriptor>(),
				new DaprClientBuilder().Build());
		}

		return builder;
	}
}