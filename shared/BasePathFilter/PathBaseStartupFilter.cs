namespace BasePathFilter;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

public class PathBaseStartupFilter : IStartupFilter
{
	private readonly string _pathBase;

	/// <summary>
	/// Initializes a new instance of the <see cref="PathBaseStartupFilter"/> class.
	/// </summary>
	/// <param name="options">Filter Options.</param>
	public PathBaseStartupFilter(IOptions<PathBaseSettings> options)
	{
		_pathBase = options.Value.ApplicationPathBase;
	}

	/// <inheritdoc />
	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
	{
		return app =>
		{
			app.UsePathBase(_pathBase);
			next(app);
		};
	}
}