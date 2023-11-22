    using SharpNeat;
    using Godot;

    #pragma warning disable

    public sealed class Evaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        public GamePool GamePool {set; get;}

        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            GamePool.StartGame();
            GD.Print("Evaluator created game.");

            double fitness = 0.0;
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

        #region Private Static Methods

        private static double Activate(IBlackBox<double> box, double in1, double in2)
        {
            var inputs = box.Inputs.Span;
            var outputs = box.Outputs.Span;

            // Bias input.
            inputs[0] = 1.0;

            // XOR inputs.
            inputs[1] = in1;
            inputs[2] = in2;

            // Activate the black box.
            box.Activate();

            // Read output signal.
            double output = outputs[0];
            Clip(ref output);
            // Debug.Assert(output >= 0.0, "Unexpected negative output.");
            return output;
        }

        private static void Clip(ref double x)
        {
            if(x < 0.0) x = 0.0;
            else if(x > 1.0) x = 1.0;
        }

        #endregion
    }