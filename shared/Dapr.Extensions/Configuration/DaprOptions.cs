// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="DaprOptions.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Dapr.Extension.Configuration;

public class DaprOptions
{
	public DaprSecret? Secrets { get; set; }
}