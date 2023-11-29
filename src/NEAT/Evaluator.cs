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
        double fitness = 0.0;

        // Game time reward
        fitness += (1.0 - results.MatchTime) * 10.0;
        
        if (results.Winner == "Tie") {
            fitness += 5.0;
        }
        // Winner is current team
        else if (results.Winner == Team) {
            fitness += 15.0;
        }
        // Winner is other team
        else {
            fitness += 0.0;
        }

        if (Team == "TeamA") {
            fitness += Math.Max(results.TeamBDmgReceived - (results.TeamADmgReceived * 0.8), 0.0) * 10.0;
            fitness += Math.Max(results.TeamBDeaths - results.TeamADeaths, 0.0) * 5.0;
        }
        else {
            fitness += Math.Max(results.TeamADmgReceived - (results.TeamBDmgReceived * 0.8), 0.0) * 10.0;
            fitness += Math.Max(results.TeamADeaths - results.TeamBDeaths, 0.0) * 5.0;
        }

        GD.Print(results + $", {Team}_Fit={fitness}");

        return new FitnessInfo(fitness);
    }

}