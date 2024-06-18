using System.ComponentModel;

public class Game
{
	public string url { get; set; }
	public string pgn { get; set; }
	public string time_control { get; set; }
	public int end_time { get; set; }
	public bool rated { get; set; }
	public string tcn { get; set; }
	public string uuid { get; set; }
	public string initial_setup { get; set; }
	public string fen { get; set; }
	public string time_class { get; set; }
	public string rules { get; set; }
	public White white { get; set; }
	public Black black { get; set; }
	//public Accuracies accuracies { get; set; }
}

