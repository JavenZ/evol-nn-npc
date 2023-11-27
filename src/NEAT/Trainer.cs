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

        // Instantiate game pool
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

        // Initialize algorithm for each team
        await Parallel.ForEachAsync(
            teams,
            new ParallelOptions { MaxDegreeOfParallelism = 2 },
            async (team, ct) =>
            {
                var ea = await InitializeAlgorithm(team);
                
                mutex.Lock();
                algorithms.Add(ea);
                mutex.Unlock();
            });
        
        foreach (var ea in algorithms)
        {
            var neatPop = ea.Population;
            GD.Print(neatPop.Stats);
        }
        GD.Print(algorithms.Count);

        // // Create the initial population
        // var neatPop = ea.Population;
        // GD.Print(neatPop.Stats);

        // for(int i = 0; i < 0; i++)
        // {
        //     // Initialize game pool
        //     GamePool.Initialize();

        //     // Evaluate generation
        //     await ea.PerformOneGeneration();
        //     GD.Print($"Gen[{ea.Stats.Generation}] Fit_Best={neatPop.Stats.BestFitness.PrimaryFitness}, Fit_Mean={neatPop.Stats.MeanFitness}, Complexity_Mean={neatPop.Stats.MeanComplexity}, Complexity_Mode={ea.ComplexityRegulationMode}");

        //     // if(ea.Population.Stats.BestFitness.PrimaryFitness >= 14.0)
        //     // {
        //     //     break;
        //     // }
        // }
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

    // private async Task<Population> RunAlgorithm(NeatEvolutionAlgorithm<Double> ea)
    // {

    // }
    
}

