// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePathStartupFilter.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.AspNetCore.BasePathFilter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

/// <summary>
/// ASP.NET Startup Filter which intercepts all incoming requests and handles appending a base url prefix to the request
/// similar to base paths when working in JS frameworks.
/// </summary>
public class BasePathStartupFilter : IStartupFilter
{
	private readonly IOptionsMonitor<BasePathOptions> _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="BasePathStartupFilter"/> class.
	/// </summary>
	/// <param name="options">Filter Options.</param>
	public BasePathStartupFilter(IOptionsMonitor<BasePathOptions> options)
		=> _options = options;

	/// <inheritdoc />
	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		=> app =>
		{
			app.UsePathBase(_options.CurrentValue.ApplicationBasePath);
			next(app);
		};
}