// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingBaseOptions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Bindings;

/// <summary>
/// Base options for Dapr bindings.
/// </summary>
public abstract class BindingBaseOptions
{
	/// <summary>
	/// Gets the Dapr operation type to perform.
	/// </summary>
	public virtual string Operation { get; } = string.Empty;

	/// <summary>
	/// Gets or sets the name of the Dapr binding to invoke.
	/// </summary>
	public string BindingName { get; set; } = string.Empty;
}