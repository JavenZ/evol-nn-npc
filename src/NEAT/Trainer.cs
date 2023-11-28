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
    [Export]
    public int Generations = 10;

    public static GamePool GamePool;

    public override void _Ready()
    {
        GD.Print($"Trainer Ready()");

        // Instantiate shared game pool
        GamePool = new GamePool()
        {
            Trainer=this,
            Size=PopulationSize,
        };
        GamePool.Initialize();

        // Start training process
        train();
    }

    private async void train()
    {
        List<NeatEvolutionAlgorithm<Double>> algorithms = new List<NeatEvolutionAlgorithm<Double>>(2);
        Godot.Mutex mutex = new Godot.Mutex();
        string[] teams = {"TeamA", "TeamB"};
        ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 2 };

        // Initialize algorithms concurrently for each team
        await Parallel.ForEachAsync(
            teams,
            parallelOptions,
            async (team, ct) =>
            {
                // Initialize algorithm
                var ea = await InitializeAlgorithm(team);
                
                // Add to list of initialized algorithms
                mutex.Lock();
                algorithms.Add(ea);
                mutex.Unlock();
            });
        
        // Run each algorithm for each generation
        for (int i = 0; i < Generations; i++)
        {
            // Reset shared game pool
            GamePool.Reset();

            // Run concurrently for each team
            await Parallel.ForEachAsync(
                algorithms,
                parallelOptions,
                async (ea, ct) =>
                {
                    await RunAlgorithm(ea);
                });
        }

        // Finished!
        GD.Print("Training finished!");
    }

    private async Task<NeatEvolutionAlgorithm<Double>> InitializeAlgorithm(String team)
    {
        GD.Print($"Initializing the EA for {team}...");
        // Experiment ID
        var Id = team;

        // Create the evaluation scheme
        var evalScheme = new EvaluationScheme()
        {
            GamePool=GamePool,
            Team=team,
        };
        // GD.Print("Initialized evaluation scheme.");
        
        // Create a NeatExperiment object with the evaluation scheme
        var experiment = new NeatExperiment<double>(evalScheme, Id)
        {
            IsAcyclic = true,
            ActivationFnName = ActivationFunctionId.LeakyReLU.ToString(),
            PopulationSize = PopulationSize,
        };
        // GD.Print("Initialized experiment.");

        // Create a NeatEvolutionAlgorithm instance ready to run the experiment
        var ea = NeatUtils.CreateNeatEvolutionAlgorithm(experiment);
        await ea.Initialise();
        GD.Print($"Initialized the EA for {team}!");

        return ea;
    }

    private async Task<NeatPopulation<Double>> RunAlgorithm(NeatEvolutionAlgorithm<Double> ea)
    {
        // Evaluate generation
        await ea.PerformOneGeneration();
        var neatPop = ea.Population;
        GD.Print($"Gen[{ea.Stats.Generation}] Fit_Best={neatPop.Stats.BestFitness.PrimaryFitness}, Fit_Mean={neatPop.Stats.MeanFitness}, Complexity_Mean={neatPop.Stats.MeanComplexity}, Size={neatPop.TargetSize}, Complexity_Mode={ea.ComplexityRegulationMode}");         
        return neatPop;
    }
    
}

