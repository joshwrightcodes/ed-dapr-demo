// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="SmtpBindingOptions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Bindings.Smtp;

public class SmtpBindingOptions : BindingBaseOptions
{
	/// <inheritdoc />
	public override string Operation => "create";
}