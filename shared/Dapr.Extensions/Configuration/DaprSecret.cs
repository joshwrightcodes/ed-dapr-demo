namespace DaprDemo.Dapr.Extension.Configuration;

public class DaprSecret
{
	public string? Store { get; set; }

	public List<string>? Descriptors { get; set; }
}