using Godot;
using Godot.Collections;
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
using Godot.Bridge;
using Godot.NativeInterop;
using Microsoft.VisualBasic;
using System.Collections;

#pragma warning disable

[GlobalClass]
public partial class Trainer : Node
{
    // Member variables here, example:
    [Export]
    public int PopulationSize = 10;

    public static GamePool GamePool;

    public override void _Ready()
    {
        GD.Print($"Trainer Ready()");

        // Initialize game pool
        GamePool = new GamePool() { trainer=this };
        GamePool.Initialize(PopulationSize);

        // Start training process
        train();
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since the last frame.
        // Update game logic here.
    }

    public void train()
    {
        // Experiment ID
        var Id = "Test";

        // Create the evaluation scheme
        var evalScheme = new EvaluationScheme()
        {
            GamePool=GamePool,
        };
        GD.Print("Initialized evaluation scheme.");
        
        // Create a NeatExperiment object with the evaluation scheme
        var experiment = new NeatExperiment<double>(evalScheme, Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString(),
            PopulationSize = PopulationSize,
        };
        GD.Print("Initialized experiment.");

        // Create a NeatEvolutionAlgorithm instance ready to run the experiment
        var ea = NeatUtils.CreateNeatEvolutionAlgorithm(experiment);
        ea.Initialise();
        GD.Print("Initialized the evolutionary algorithm.");

        // Create the initial population
        var neatPop = ea.Population;
        GD.Print(neatPop.Stats);

        for(int i = 0; i < 0; i++)
        {
            // Initialize game pool
            GamePool.Initialize(PopulationSize);

            // Evaluate generation
            ea.PerformOneGeneration();
            GD.Print($"Gen[{ea.Stats.Generation}] Fit_Best={neatPop.Stats.BestFitness.PrimaryFitness}, Fit_Mean={neatPop.Stats.MeanFitness}, Complexity_Mean={neatPop.Stats.MeanComplexity}, Complexity_Mode={ea.ComplexityRegulationMode}");

            if(ea.Population.Stats.BestFitness.PrimaryFitness >= 14.0)
            {
                break;
            }
        }
    }
    
}

