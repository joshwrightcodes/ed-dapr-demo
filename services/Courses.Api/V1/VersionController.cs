// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionController.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api.V1;

using DaprDemo.AspNetCore.BaseController;
using Microsoft.AspNetCore.Mvc;

public partial class VersionController : BaseController
{
	private readonly ILogger<VersionController> _logger;
	private readonly IConfiguration _configuration;

	public VersionController(ILogger<VersionController> logger, IConfiguration configuration)
	{
		_logger = logger;
		_configuration = configuration;
	}

	[HttpGet]
	public ActionResult<Dictionary<string, string?>> GetVersion()
	{
		GetVersionLog();
		return new Dictionary<string, string?>
		{
			["AssemblyInformationalVersion"] = _configuration.GetValue<string>("APP_VERSION"),
		};
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Sending Api Version Metadata")]
	public partial void GetVersionLog();
}