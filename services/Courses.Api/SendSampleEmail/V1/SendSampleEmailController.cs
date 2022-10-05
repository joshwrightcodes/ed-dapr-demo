// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="SendSampleEmailController.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api.SendSampleEmail.V1;

using System.Net;
using System.Reflection;
using DaprDemo.AspNetCore.BaseController;
using DaprDemo.Dapr.Extension.Bindings.Smtp;
using global::Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public partial class MailController : BaseController
{
	/// <summary>
	/// Gets the name of the options section used for this controller.
	/// </summary>
	public const string SmtpOptionsName = nameof(SmtpBindingOptions);

	private static readonly Action<ILogger, string, Exception> LogSendEmail =
		LoggerMessage.Define<string>(
			LogLevel.Information,
			new EventId(0, nameof(Send)),
			"Sending email to {EmailAddress}");

	private readonly ILogger<MailController> _logger;
	private readonly IOptionsMonitor<SmtpBindingOptions> _options;
	private readonly IConfiguration _configuration;
	private readonly DaprClient _daprClient;

	/// <summary>
	/// Initializes a new instance of the <see cref="MailController"/> class.
	/// </summary>
	/// <param name="logger">Logging instance.</param>
	/// <param name="options">Controller Options.</param>
	/// <param name="configuration">Configuration object to retrieve application version from.</param>
	/// <param name="daprClient">Dapr Client instance.</param>
	public MailController(
		ILogger<MailController> logger,
		IOptionsMonitor<SmtpBindingOptions> options,
		IConfiguration configuration,
		DaprClient daprClient)
	{
		_logger = logger;
		_options = options;
		_configuration = configuration;
		_daprClient = daprClient;
	}

	[HttpPost("[action]")]
	public async Task<IActionResult> Send(
		[FromBody] SendDto data,
		CancellationToken cancellationToken = default)
	{
		SmtpBindingOptions? opts = _options.Get(SmtpOptionsName);

		LogSendEmail(_logger, data.EmailAddress, null!);

		await _daprClient.InvokeBindingAsync(
			opts.BindingName,
			opts.Operation,
			$"<html><body><p>Hello <b>{data.Name}</b>!</p><p>Email sent from service {Assembly.GetExecutingAssembly().GetName().Name!} ({_configuration.GetValue<string>("APP_VERSION")}) on host {Dns.GetHostName()}.</p></body><html>",
			new Dictionary<string, string>
			{
				["emailTo"] = data.EmailAddress,
				["subject"] = $"Hello {data.Name}!",
			},
			cancellationToken);

		return Ok();
	}
}