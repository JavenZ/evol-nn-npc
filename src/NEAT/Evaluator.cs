    using SharpNeat;
    using Godot;

    #pragma warning disable

    public sealed class Evaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        public GamePool GamePool {set; get;}

        public async Task<FitnessInfo> Evaluate(IBlackBox<double> box)
        {
            // double fitness = await GamePool.StartGame();
            double fitness = await GamePool.StartGame(box);

            // bool success = true;

            // // Test case 0, 0.
            // double output = Activate(box, 0.0, 0.0);
            // success &= output <= 0.5;
            // fitness += 1.0 - (output * output);

            // // Test case 1, 1.
            // box.Reset();
            // output = Activate(box, 1.0, 1.0);
            // success &= output <= 0.5;
            // fitness += 1.0 - (output * output);

            // // Test case 0, 1.
            // box.Reset();
            // output = Activate(box, 0.0, 1.0);
            // success &= output > 0.5;
            // fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // // Test case 1, 0.
            // box.Reset();
            // output = Activate(box, 1.0, 0.0);
            // success &= output > 0.5;
            // fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // // If all four responses were correct then we add 10 to the fitness.
            // if(success)
            //     fitness += 10.0;

            return new FitnessInfo(fitness);
        }

    }