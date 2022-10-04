// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Configuration;

using global::Dapr.Client;
using global::Dapr.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Dapr in a consistent manner.
/// </summary>
public static class ConfigurationExtensions
{
	/// <summary>
	/// Adds Dapr Services to the service collection including <see cref="DaprClient"/>.
	/// </summary>
	/// <param name="services"><see cref="IServiceCollection"/> to register Dapr services with.</param>
	/// <returns>Updated <see cref="IServiceCollection"/> with <see cref="DaprClient"/>.</returns>
	public static IServiceCollection AddDaprServices(this IServiceCollection services)
	{
		services.AddDaprClient();

		return services;
	}

	/// <summary>
	/// Adds Configuration Provider using Dapr's configuration and secrets building blocks. Extension uses an existing
	/// config bound to <see cref="DaprOptions"/>. If this is not found then Dapr's configuration provider is not bound.
	/// </summary>
	/// <param name="configurationManager">Application's instance of <see cref="ConfigurationManager"/>.</param>
	/// <param name="serviceCollection">Services.</param>
	/// <param name="section">Name of configuration section to bind to <see cref="DaprOptions"/>.</param>
	/// <returns>Updated <see cref="ConfigurationManager"/> object with Dapr Configuration provider if configured, otherwise unchanged.</returns>
	public static ConfigurationManager AddDaprConfiguration(
		this ConfigurationManager configurationManager,
		IServiceCollection serviceCollection,
		string section = nameof(DaprOptions))
	{
		var options = configurationManager.GetSection(section).Get<DaprOptions>();

		if (options?.Secrets != null && !string.IsNullOrWhiteSpace(options.Secrets.Store))
		{
			configurationManager.AddDaprSecretStore(
				options.Secrets.Store,
				options.Secrets.Descriptors?.Select(sd => new DaprSecretDescriptor(sd)) ?? new List<DaprSecretDescriptor>(),
				serviceCollection.BuildServiceProvider().GetRequiredService<DaprClient>());
		}

		return configurationManager;
	}
}