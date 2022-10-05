// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingOptionsBaseValidator.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Bindings;

using DaprDemo.FluentValidation.Extension;
using global::FluentValidation;

/// <summary>
/// Base validation for classes inheriting from <see cref="BindingBaseOptions"/>.
/// </summary>
/// <typeparam name="T">Derived type inheriting from <see cref="BindingBaseOptions"/>.</typeparam>
public abstract class BindingOptionsBaseValidator<T> : AbstractOptionsValidator<T>
	where T : BindingBaseOptions
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BindingOptionsBaseValidator{T}"/> class.
	/// <para>Validation Rules:
	/// <list type="bullet">
	/// <value><c>Operation</c> must not be empty</value>
	/// <value><c>BindingName</c> must not be empty</value>
	/// </list>
	/// </para>
	/// </summary>
	protected BindingOptionsBaseValidator()
	{
		RuleFor(p => p.Operation)
			.NotEmpty();

		RuleFor(p => p.BindingName)
			.NotEmpty();
	}
}