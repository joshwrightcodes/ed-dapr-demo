// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.OpenTelemetry.Extensions;

using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for configuring OpenTelemetry services.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Registers required OTLP configurations for Metrics and Traces.
	/// </summary>
	/// <param name="services"><see cref="IServiceCollection"/> to register OpenTelemetry services with.</param>
	/// <param name="resourceBuilder">OpenTelemetry resource configuration. If null, uses the SDK default.</param>
	/// <returns>Updated <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddOpenTelemetryServices(
		this IServiceCollection services,
		ResourceBuilder? resourceBuilder = null)
	{
		ArgumentNullException.ThrowIfNull(services);

		resourceBuilder ??= ResourceBuilder.CreateDefault();

		services.AddOpenTelemetryMetrics(options =>
		{
			options.SetResourceBuilder(resourceBuilder)
				.AddRuntimeInstrumentation()
				.AddHttpClientInstrumentation()
				.AddAspNetCoreInstrumentation()
				.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc);
		});

		services.AddOpenTelemetryTracing(options =>
		{
			options.SetResourceBuilder(resourceBuilder)
				.AddSource()
				.SetSampler(new AlwaysOnSampler())
				.AddHttpClientInstrumentation()
				.AddAspNetCoreInstrumentation()
				.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc);
		});

		return services;
	}
}