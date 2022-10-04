// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="MailController.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.Courses.Api.V1;

using System.Net;
using System.Reflection;
using DaprDemo.AspNetCore.BaseController;
using DaprDemo.Dapr.Extension.Bindings.Smtp;
using global::Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public partial class MailController : BaseController
{
	private readonly ILogger _logger;
	private readonly IOptionsMonitor<SmtpOptions> _smtpOptions;
	private readonly DaprClient _daprClient;

	public MailController(
		ILogger<MailController> logger,
		IOptionsMonitor<SmtpOptions> smtpOptions,
		DaprClient daprClient)
	{
		_logger = logger;
		_smtpOptions = smtpOptions;
		_daprClient = daprClient;
	}

	[HttpGet("[action]")]
	public IActionResult Send(
		[FromQuery] string email,
		[FromQuery] string name,
		CancellationToken cancellationToken = default)
	{
		SendMailLog(email);

		return _daprClient.InvokeBindingAsync(
			"dapr-demo-users-api-sendmail",
			"create",
			$"<html><body><p>Hello <b>{name}</b>!</p><p>Email sent from service {Assembly.GetExecutingAssembly().GetName().Name!} ({configuration.GetValue<string>("APP_VERSION")}) on host {Dns.GetHostName()}.</p></body><html>",
			new Dictionary<string, string>
			{
				["emailFrom"] = $"{Dns.GetHostName()}@daprdemo.wright.codes",
				["emailTo"] = email,
				["subject"] = $"Hello {name}!",
			},
			cancellationToken);
	}

	[LoggerMessage(Message = "Sending email to {EmailAddress}", Level = LogLevel.Information)]
	public partial void SendMailLog(string email);
}