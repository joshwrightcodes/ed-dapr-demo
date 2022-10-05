// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Bindings.Smtp;

using global::FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Extensions for registering a Dapr SMTP binding.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds a new SMTP binding for the given section name. Options are bound as named options using the value specified
	/// in <paramref name="sectionName"/>, which defaults to <c>SmtpBindingOptions</c>.
	/// </summary>
	/// <param name="services">
	/// <see cref="IServiceCollection"/> to register services and options with.
	/// </param>
	/// <param name="configuration">
	/// Configuration to obtain the binding configuration from.
	/// </param>
	/// <param name="sectionName">
	/// Configuration section name to obtain binding configuration from. Defaults to <c>SmtpBindingOptions</c>.
	/// </param>
	/// <returns>Updated <paramref name="services"/>.</returns>
	/// <exception cref="ArgumentException">
	/// Thrown when <paramref name="sectionName"/> is null, empty or whitespace.
	/// </exception>
	/// <exception cref="ArgumentNullException">
	/// Thrown when <paramref name="services"/> or <paramref name="configuration"/> is null.
	/// </exception>
	public static IServiceCollection AddDaprSmtpBinding(
		this IServiceCollection services,
		IConfiguration configuration,
		string sectionName = nameof(SmtpBindingOptions))
	{
		ArgumentNullException.ThrowIfNull(services);
		ArgumentNullException.ThrowIfNull(configuration);

		if (string.IsNullOrWhiteSpace(sectionName))
		{
			throw new ArgumentException("Value cannot be null or whitespace.", sectionName);
		}

		services.AddSingleton<SmtpBindingOptionsValidator>();
		services.AddSingleton<IValidator<SmtpBindingOptions>, SmtpBindingOptionsValidator>();
		services.AddSingleton<IValidateOptions<SmtpBindingOptions>, SmtpBindingOptionsValidator>();

		services.AddOptions()
			.AddOptions<SmtpBindingOptions>(sectionName)
			.BindConfiguration(sectionName)
			.ValidateOnStart();

		return services;
	}
}