// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="HelloController.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api.V1;

using System.Net.Mime;
using System.Reflection;
using DaprDemo.AspNetCore.BaseController;
using Microsoft.AspNetCore.Mvc;

public partial class HelloController : BaseController
{
	private static readonly Action<ILogger, Exception> LogSayHello =
		LoggerMessage.Define(
			LogLevel.Information,
			new EventId(0, nameof(SayHello)),
			"Saying Hello");

	private readonly ILogger<HelloController> _logger;

	public HelloController(ILogger<HelloController> logger)
	{
		_logger = logger;
	}

	[HttpGet]
	[Produces(MediaTypeNames.Text.Plain)]
	public ActionResult<string> SayHello()
	{
		LogSayHello(_logger, null!);
		return $"Hello from {Assembly.GetExecutingAssembly().GetName().Name!}!";
	}
}