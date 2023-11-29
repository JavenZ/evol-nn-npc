using Godot;
using System;

[GlobalClass]
public partial class GameResults : Resource
{
	public string GameID {set; get;}
	public string Winner {set; get;}
	public float MatchTime {set; get;}
	public float TeamADmgReceived {set; get;}
	public float TeamBDmgReceived {set; get;}
	public int TeamADeaths {set; get;}
	public int TeamBDeaths {set; get;}

	public override string ToString()
	{
		return $"GameResults: ID={GameID}, winner={Winner}, MatchTime={MatchTime}, TeamA=[{TeamADmgReceived}, {TeamADeaths}], TeamB=[{TeamBDmgReceived}, {TeamBDeaths}]";
	}
}
