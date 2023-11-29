using SharpNeat;
using Godot;

#pragma warning disable

public sealed class Evaluator : IPhenomeEvaluator<IBlackBox<double>>
{
    public GamePool GamePool {set; get;}

    public String Team {set; get;}

    const double WIN_REWARD = 10.0;
    const double TIME_REWARD = 8.0;
    const double HEALTH_REWARD = 7.5;
    const double DMG_REWARD = 7.5;
    const double KILL_REWARD = 5.0;

    public async Task<FitnessInfo> Evaluate(IBlackBox<double> box)
    {
        var results = await GamePool.JoinGame(box, Team);
        double fitness = 0.0;

        // Game time reward
        fitness += (1.0 - results.MatchTime) * TIME_REWARD;
        
        // Winner is current team
        if (results.Winner == Team) {
            fitness += WIN_REWARD;
        }

        if (Team == "TeamA") {
            fitness += (1.0 - results.TeamADmgReceived) * HEALTH_REWARD;
            fitness += results.TeamBDmgReceived * DMG_REWARD;
            fitness += results.TeamBDeaths * KILL_REWARD;
        }
        else if (Team == "TeamB") {
            fitness += (1.0 - results.TeamBDmgReceived) * HEALTH_REWARD;
            fitness += results.TeamADmgReceived * DMG_REWARD;
            fitness += results.TeamADeaths * KILL_REWARD;
        }

        GD.Print(results + $", {Team}_Fit={fitness}");

        return new FitnessInfo(fitness);
    }

}