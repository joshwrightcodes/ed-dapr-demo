// ---------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Josh Wright">
// Copyright 2022 Josh Wright. Use of this source code is governed by an MIT-style, license that can be found in the
// LICENSE file or at https://opensource.org/licenses/MIT.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------------

namespace DaprDemo.AspNetCore.BaseController;

using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Base controller that specifies route and produce/consume mediatypes.
/// </summary>
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Consumes(MediaTypeNames.Application.Json)]
[Route("[controller]")]
public abstract class BaseController : ControllerBase
{
}