// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="DaprHealthCheck.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Shared.HealthChecks.DaprHealth;

using Dapr.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class DaprHealthCheck : IHealthCheck
{
	private readonly DaprClient _daprClient;

	public DaprHealthCheck(DaprClient daprClient)
		=> _daprClient = daprClient;

	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		var healthy = await _daprClient.CheckHealthAsync(cancellationToken);

		return healthy
			? HealthCheckResult.Healthy("Dapr sidecar is healthy.")
			: new HealthCheckResult(context.Registration.FailureStatus, "Dapr sidecar is unhealthy.");
	}
}