using Godot;
using SharpNeat.Experiments;
using SharpNeat.Experiments.ConfigModels;
using SharpNeat.IO;
using SharpNeat.NeuralNets;
using SharpNeat.Evaluation;
using SharpNeat.EvolutionAlgorithm;
﻿using SharpNeat.Neat.EvolutionAlgorithm;
using SharpNeat.Neat;
using SharpNeat;
using System.Diagnostics;

public partial class Trainer : Node
{
    // Member variables here, example:

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here.
        var a = "tests";
        GD.Print($"Hello {a} from C# to Godot :)");

        // test();
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since the last frame.
        // Update game logic here.
    }

    public void test()
    {
        // Experiment ID
        var Id = "Test";

        // Evaluation Scheme
        var evalScheme = new EvaluationScheme();
        GD.Print("Initialized evaluation scheme.");
        
        // Create a NeatExperiment object with the evaluation scheme,
        // and assign some default settings (these can be overridden by config).
        var experiment = new NeatExperiment<double>(evalScheme, Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString(),
            PopulationSize = 100,
        };
        GD.Print("Initialized experiment.");

        // Create a NeatEvolutionAlgorithm instance ready to run the experiment.
        var ea = NeatUtils.CreateNeatEvolutionAlgorithm(experiment);
        ea.Initialise();
        GD.Print("Initialized the evolutionary algorithm.");

        // Create the initial population
        var neatPop = ea.Population;
        GD.Print(neatPop.Stats);

        for(int i = 0; i < 10_000; i++)
        {
            ea.PerformOneGeneration();
            GD.Print($"Gen[{ea.Stats.Generation}] Fit_Best={neatPop.Stats.BestFitness.PrimaryFitness}, Fit_Mean={neatPop.Stats.MeanFitness}, Complexity_Mean={neatPop.Stats.MeanComplexity}, Complexity_Mode={ea.ComplexityRegulationMode}");

            if(ea.Population.Stats.BestFitness.PrimaryFitness >= 14.0)
            {
                break;
            }
        }
    }

    public sealed class EvaluationScheme : IBlackBoxEvaluationScheme<double>
    {
        /// <inheritdoc/>
        public int InputCount => 3;

        /// <inheritdoc/>
        public int OutputCount => 1;

        /// <inheritdoc/>
        public bool IsDeterministic => true;

        /// <inheritdoc/>
        public IComparer<FitnessInfo> FitnessComparer => PrimaryFitnessInfoComparer.Singleton;

        /// <inheritdoc/>
        public FitnessInfo NullFitness => FitnessInfo.DefaultFitnessInfo;

        /// <inheritdoc/>
        public bool EvaluatorsHaveState => false;

        /// <inheritdoc/>
        public IPhenomeEvaluator<IBlackBox<double>> CreateEvaluator()
        {
            return new Evaluator();
        }

        /// <inheritdoc/>
        public bool TestForStopCondition(FitnessInfo fitnessInfo)
        {
            return (fitnessInfo.PrimaryFitness >= 10);
        }
    }

    public sealed class Evaluator : IPhenomeEvaluator<IBlackBox<double>>
    {
        public FitnessInfo Evaluate(IBlackBox<double> box)
        {
            double fitness = 0.0;
            bool success = true;

            // Test case 0, 0.
            double output = Activate(box, 0.0, 0.0);
            success &= output <= 0.5;
            fitness += 1.0 - (output * output);

            // Test case 1, 1.
            box.Reset();
            output = Activate(box, 1.0, 1.0);
            success &= output <= 0.5;
            fitness += 1.0 - (output * output);

            // Test case 0, 1.
            box.Reset();
            output = Activate(box, 0.0, 1.0);
            success &= output > 0.5;
            fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // Test case 1, 0.
            box.Reset();
            output = Activate(box, 1.0, 0.0);
            success &= output > 0.5;
            fitness += 1.0 - ((1.0 - output) * (1.0 - output));

            // If all four responses were correct then we add 10 to the fitness.
            if(success)
                fitness += 10.0;

            return new FitnessInfo(fitness);
        }

        #region Private Static Methods

        private static double Activate(
            IBlackBox<double> box,
            double in1, double in2)
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
            Debug.Assert(output >= 0.0, "Unexpected negative output.");
            return output;
        }

        private static void Clip(ref double x)
        {
            if(x < 0.0) x = 0.0;
            else if(x > 1.0) x = 1.0;
        }

        #endregion
    }

    
}

