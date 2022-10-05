// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractOptionsValidator.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.FluentValidation.Extension;

using global::FluentValidation;
using global::FluentValidation.Results;
using Microsoft.Extensions.Options;

/// <summary>
/// Base validator for using FluentValidation with .NET native <see cref="IOptions{TOptions}"/> and
/// <see cref="IValidateOptions{TOptions}"/>.
/// </summary>
/// <typeparam name="T">Options object type for validation.</typeparam>
public class AbstractOptionsValidator<T> : AbstractValidator<T>, IValidateOptions<T>
	where T : class
{
	/// <inheritdoc />
	public ValidateOptionsResult Validate(string name, T options)
	{
		ValidationResult? validateResult = this.Validate(options);
		return validateResult.IsValid
			? ValidateOptionsResult.Success
			: ValidateOptionsResult.Fail(validateResult.Errors.Select(x => x.ErrorMessage));
	}
}