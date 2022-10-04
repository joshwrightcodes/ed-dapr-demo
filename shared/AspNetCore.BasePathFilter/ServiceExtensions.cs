// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.AspNetCore.BasePathFilter;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering <see cref="BasePathStartupFilter"/> and its associated dependencies in .NET
/// applications.
/// </summary>
public static class ServiceExtensions
{
	/// <summary>
	/// Add and configure Base Path middleware to the ASP.NET Core Startup pipeline.
	/// This middleware appends a base path to all requests to handle incoming requests from reverse proxies using
	/// prefixes to serve the service from a single domain.
	/// Configuration is stored under <c>BasePathOptions</c> in the root of the configuration. Refer to
	/// <see cref="BasePathOptions"/> for more details on the options available for the middleware.
	/// </summary>
	/// <param name="services">Service Collection to register dependencies with.</param>
	/// <param name="configuration">Configuration section to retrieve the middleware configuration from.</param>
	/// <returns>Updated Service Collection.</returns>
	public static IServiceCollection AddBasePathMiddleware(this IServiceCollection services, IConfiguration configuration)
		=> services
			.Configure<BasePathOptions>(configuration.GetSection(nameof(BasePathOptions)))
			.AddTransient<IStartupFilter, BasePathStartupFilter>();
}