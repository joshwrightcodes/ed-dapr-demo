// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePathOptions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Shared.BasePathFilter;

/// <summary>
/// Options for configuring <see cref="BasePathStartupFilter"/>.
/// </summary>
public class BasePathOptions
{
	/// <summary>
	/// Gets or sets a string to be appended to the beginning of each request as it flows through the ASP.NET core
	/// middleware.
	/// </summary>
	/// <example>
	/// `ApplicationBasePath` set to `users`, Api Route of `GET groups` becomes `GET users/groups` for incoming calls.
	/// </example>
	public string? ApplicationBasePath { get; set; }
}