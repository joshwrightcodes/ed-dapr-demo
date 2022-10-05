// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApplicationBuilderExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api;

using System.Reflection;
using DaprDemo.AspNetCore.BasePathFilter;
using DaprDemo.Courses.Api.SendSampleEmail.V1;
using DaprDemo.Dapr.Extension.Bindings.Smtp;
using DaprDemo.Dapr.Extension.Configuration;
using DaprDemo.Dapr.Extension.HealthChecks;
using DaprDemo.OpenApi.Extensions;
using DaprDemo.OpenTelemetry.Extensions;
using global::OpenTelemetry.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

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

		builder.Services.AddDaprSmtpBinding(builder.Configuration, MailController.SmtpOptionsName);

		return builder;
	}

	public static WebApplicationBuilder AddBasePathMiddleware(this WebApplicationBuilder builder)
	{
		builder.Services.AddBasePathMiddleware(builder.Configuration);
		return builder;
	}

	public static WebApplicationBuilder AddApiVersioning(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddApiVersioning(options =>
			{
				options.ReportApiVersions = true;
				options.ApiVersionReader = ApiVersionReader.Combine(
					new MediaTypeApiVersionReader(), // Preferred Versioning
					new QueryStringApiVersionReader()); // Add this one for the HATEOAS stuff to stop complaining
				options.AssumeDefaultVersionWhenUnspecified = true;
				options.DefaultApiVersion = new ApiVersion(1, 0);
				options.Conventions.Add(new VersionByNamespaceConvention());
			})
			.AddVersionedApiExplorer(options =>
			{
				// add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
				// note: the specified format code will format the version as "'v'major[.minor][-status]"
				options.GroupNameFormat = "'v'VVV";

				// note: this option is only necessary when versioning by url segment. the SubstitutionFormat
				// can also be used to control the format of the API version in route templates
				options.SubstituteApiVersionInUrl = true;
			})
			.AddEndpointsApiExplorer();

		return builder;
	}

	public static WebApplicationBuilder AddOpenApi(this WebApplicationBuilder builder)
	{
		builder.Services
			.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", builder.Configuration.GetValue<OpenApiInfo>($"{Assembly.GetExecutingAssembly().GetName()}.v1"));

				string filePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
				options.IncludeXmlComments(filePath);

				options.OperationFilter<ApiVersionFilter>();

				// https://github.com/mattfrear/Swashbuckle.AspNetCore.Filters
				options.OperationFilter<AddHeaderOperationFilter>(
					"traceparent",
					"HTTP header field identifies the incoming request in a tracing system",
					false);
				options.OperationFilter<AddHeaderOperationFilter>(
					"tracestate",
					"HTTP header to provide additional vendor-specific trace identification information across different distributed tracing systems and is a companion header for the `traceparent` field",
					false);

				// add Security information to each operation for OAuth2
				options.OperationFilter<SecurityRequirementsOperationFilter>();
			});

		return builder;
	}
}