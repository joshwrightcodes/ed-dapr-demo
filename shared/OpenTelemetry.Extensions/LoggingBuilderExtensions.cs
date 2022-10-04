// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingBuilderExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.OpenTelemetry.Extensions;

using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Resources;
using Microsoft.Extensions.Logging;

/// <summary>
/// Extensions for configuring OpenTelemetry services.
/// </summary>
public static class LoggingBuilderExtensions
{
	/// <summary>
	/// Registers required OTLP configurations for Logging.
	/// </summary>
	/// <param name="loggingBuilder"><see cref="ILoggingBuilder"/> to register OpenTelemetry logging with.</param>
	/// <param name="resourceBuilder">OpenTelemetry resource configuration. If null, uses the SDK default.</param>
	/// <returns>Updated <see cref="ILoggingBuilder"/>.</returns>
	public static ILoggingBuilder AddOpenTelemetryLogging(
		this ILoggingBuilder loggingBuilder,
		ResourceBuilder? resourceBuilder = null)
	{
		ArgumentNullException.ThrowIfNull(loggingBuilder);

		return loggingBuilder.AddOpenTelemetry(options =>
		{
			options
				.SetResourceBuilder(resourceBuilder ?? ResourceBuilder.CreateDefault())
				.AddOtlpExporter(opt => opt.Protocol = OtlpExportProtocol.Grpc);

			options.IncludeScopes = true;
			options.IncludeFormattedMessage = true;
		});
	}
}