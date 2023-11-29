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
            // GD.Print(results);
            double fitness = 0.0;

            return new FitnessInfo(fitness);
        }

    }