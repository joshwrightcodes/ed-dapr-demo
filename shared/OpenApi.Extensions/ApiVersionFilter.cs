// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiVersionFilter.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.OpenApi.Extensions;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ApiVersionFilter : IOperationFilter
{
	private const string ApiVersionQueryStringParameter = "api-version";

	/// <inheritdoc />
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		List<OpenApiParameter> qsParams = operation.Parameters
			.Where(x => x.Name.Equals(ApiVersionQueryStringParameter, StringComparison.OrdinalIgnoreCase))
			.ToList();

		foreach (OpenApiParameter? parameter in qsParams)
		{
			operation.Parameters.Remove(parameter);
		}
	}
}