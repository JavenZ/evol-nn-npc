using SharpNeat;
using Godot;

#pragma warning disable

public sealed class Evaluator : IPhenomeEvaluator<IBlackBox<double>>
{
    public GamePool GamePool {set; get;}

    public String Team {set; get;}

    const double WIN_REWARD = 5.0;
    const double TIE_REWARD = -5.0;
    const double TIME_REWARD = 8.0;
    const double HEALTH_REWARD = 10.0;
    const double DMG_REWARD = 10.0;
    const double KILL_REWARD = 0.0;

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
        else if (results.Winner == "Tie")
        {
            fitness += TIE_REWARD;
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

        // negative fitness check
        if (fitness < 0.0) fitness = 0.0;

        GD.Print(results + $", {Team}_Fit={fitness}");

        return new FitnessInfo(fitness);
    }

}