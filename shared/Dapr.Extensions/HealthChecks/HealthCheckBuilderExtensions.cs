// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthCheckBuilderExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.HealthChecks;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for configuring Dapr Health Checks.
/// </summary>
public static class HealthCheckBuilderExtensions
{
	/// <summary>
	/// Adds health checks to the API for monitoring the condition of the Dapr sidecar.
	/// </summary>
	/// <param name="builder"><see cref="IHealthChecksBuilder"/> to add Dapr health checks to.</param>
	/// <returns>Updated <see cref="IHealthChecksBuilder"/>.</returns>
	public static IHealthChecksBuilder AddDapr(this IHealthChecksBuilder builder) =>
		builder.AddCheck<DaprHealthCheck>("dapr");
}