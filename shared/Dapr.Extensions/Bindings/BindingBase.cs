// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingBase.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

using FluentValidation;

namespace DaprDemo.Dapr.Extension.Bindings;

public abstract class BindingBase
{
	protected BindingBase(string bindingName)
	{
		if (string.IsNullOrWhiteSpace(bindingName))
		{
			throw new ArgumentException("Binding name must be supplied", nameof(bindingName));
		}

		BindingName = bindingName;
	}

	public virtual string Operation { get; }

	public string BindingName { get; }
}

public abstract class BindingBaseValidator<T> : AbstractValidator<T>
	where T : BindingBase
{
	protected BindingBaseValidator()
	{
		RuleFor(p => p.Operation)
			.NotEmpty();

		RuleFor(p => p.BindingName)
			.NotEmpty();
	}
}