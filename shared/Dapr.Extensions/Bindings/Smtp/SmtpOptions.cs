// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpOptions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Bindings.Smtp;

using FluentValidation;

public class SmtpOptions
{
	public string Host { get; set; }

	public short Port { get; set; } = 25;

	public string User { get; set; }

	public string Password { get; set; }

	public bool SkipTlsVerify { get; set; }
}

/// <summary>
/// Validator for <see cref="SmtpOptions"/>.
/// </summary>
public class SmtpOptionsValidator : AbstractValidator<SmtpOptions>
{
	public SmtpOptionsValidator() =>
		RuleFor(p => p.Host)
			.NotEmpty();
}