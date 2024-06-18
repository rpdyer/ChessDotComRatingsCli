using Newtonsoft.Json;

public class Black
{
	public int rating { get; set; }
	public string result { get; set; }

	[JsonProperty("@id")]
	public string id { get; set; }
	public string username { get; set; }
	public string uuid { get; set; }
}

