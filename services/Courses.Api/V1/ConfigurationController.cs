// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationController.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api.V1;

using DaprDemo.AspNetCore.BaseController;
using Microsoft.AspNetCore.Mvc;

public partial class ConfigurationController : BaseController
{
	private static readonly Action<ILogger, Exception> LogGetConfig =
		LoggerMessage.Define(
			LogLevel.Information,
			new EventId(0, nameof(GetConfig)),
			"Getting Config Metadata");

	private readonly ILogger<ConfigurationController> _logger;
	private readonly IConfiguration _configuration;

	public ConfigurationController(ILogger<ConfigurationController> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	[HttpGet]
	public ActionResult<List<KeyValuePair<string, string>>> GetConfig()
	{
		LogGetConfig(_logger, null!);
		return _configuration.AsEnumerable().ToList();
	}
}