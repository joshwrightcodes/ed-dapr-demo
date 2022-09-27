// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace BasePathFilter;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
	public static IServiceCollection AddBasePathMiddleware(this IServiceCollection services, IConfiguration configuration)
	{
		// Bind the config section to PathBaseSettings using IOptions
		services.Configure<PathBaseSettings>(configuration.GetSection(nameof(PathBaseSettings)));

		// Register the startup filter
		services.AddTransient<IStartupFilter, PathBaseStartupFilter>();

		return services;
	}
}