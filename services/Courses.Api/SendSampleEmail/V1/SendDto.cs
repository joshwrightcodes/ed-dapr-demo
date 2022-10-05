// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="SendDto.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api.SendSampleEmail.V1;

public class SendDto
{
	public string EmailAddress { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;
}