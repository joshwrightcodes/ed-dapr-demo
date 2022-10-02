// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="DaprHealthCheckExtensions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Shared.HealthChecks.DaprHealth;

using Microsoft.Extensions.DependencyInjection;

public static class DaprHealthCheckBuilderExtensions
{
	public static IHealthChecksBuilder AddDapr(this IHealthChecksBuilder builder) =>
		builder.AddCheck<DaprHealthCheck>("dapr");
}