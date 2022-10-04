// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="DaprHealthCheck.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.HealthChecks;

using global::Dapr.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Health Check for monitoring the condition and health of the Dapr sidecar.
/// </summary>
public class DaprHealthCheck : IHealthCheck
{
	private readonly DaprClient _daprClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="DaprHealthCheck"/> class.
	/// </summary>
	/// <param name="daprClient">Instance of <see cref="DaprClient"/> to use to communicate with the sidecar.</param>
	public DaprHealthCheck(DaprClient daprClient)
		=> _daprClient = daprClient;

	/// <inheritdoc />
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