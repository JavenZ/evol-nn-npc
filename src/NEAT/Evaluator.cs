	using SharpNeat;
	using Godot;

	#pragma warning disable

	public sealed class Evaluator : IPhenomeEvaluator<IBlackBox<double>>
	{
		public GamePool GamePool {set; get;}

		public String Team {set; get;}

		public async Task<FitnessInfo> Evaluate(IBlackBox<double> box)
		{
			var results = await GamePool.JoinGame(box, Team);
			GD.Print(results);
			double fitness = 0.0;

			// Rewarding for desired game time and when humans win

			fitness = results.MatchTime >= 5 && results.MatchTime <= 15 ? 10.0 : 0.0;

			
			if (results.Winner == "Tie") {
				fitness += 10.0;
			}
			// Winner is current team
			else if (results.Winner == Team) {
				fitness += 20.0;
			} 
			// Winner is other team
			else {
				fitness += 0.0;
			}

			if (Team == "TeamA") {
				fitness += Math.Max(results.TeamBDmgReceived - results.TeamADmgReceived, 0.0);
				fitness += Math.Max(results.TeamBDeaths - results.TeamADeaths, 0.0);
			}
			else {
				fitness += Math.Max(results.TeamADmgReceived - results.TeamBDmgReceived, 0.0);
				fitness += Math.Max(results.TeamADeaths - results.TeamBDeaths, 0.0);
			}

			return new FitnessInfo(fitness);
		}

	}
