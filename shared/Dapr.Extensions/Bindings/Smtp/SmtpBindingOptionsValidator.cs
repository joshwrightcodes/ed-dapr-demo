// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpBindingOptionsValidator.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Bindings.Smtp;

using global::FluentValidation;

/// <summary>
/// Validator for <see cref="SmtpBindingOptions"/>.
/// </summary>
public class SmtpBindingOptionsValidator : BindingOptionsBaseValidator<SmtpBindingOptions>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SmtpBindingOptionsValidator"/> class.
	/// </summary>
	public SmtpBindingOptionsValidator()
	{
		RuleFor(p => p)
			.NotNull();

		RuleFor(p => p.BindingName)
			.NotEmpty();

		RuleFor(p => p.Operation)
			.NotEmpty();
	}
}