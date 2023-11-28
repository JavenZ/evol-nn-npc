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
using SharpNeat.Neat.Genome.IO;

#pragma warning disable

[GlobalClass]
public partial class Trainer : Node
{
    // Member variables here, example:
    [Export]
    public int PopulationSize = 10;
    [Export]
    public int Generations = 10;

    [Export]
    public bool LoadLatestBatch = true;

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
                SavePopulation(ea);

                // Add to list of initialized algorithms
                mutex.Lock();
                algorithms.Add(ea);
                mutex.Unlock();
            });
        GD.Print("Initialized algorithms.\n");
                
        // Run each algorithm for each generation
        for (int i = 0; i < Generations; i++)
        {
            GD.Print($"Starting Gen[{i+1}]...");
            // Reset shared game pool
            GamePool.Reset();

            // Run concurrently for each team
            await Parallel.ForEachAsync(
                algorithms,
                parallelOptions,
                async (ea, ct) =>
                {
                    await RunAlgorithm(ea);
                    SavePopulation(ea);
                });
            GD.Print($"Finished Gen[{i+1}].\n");
        }

        // Finished!
        GD.Print("\nTraining finished!");
    }

    private async Task<NeatEvolutionAlgorithm<Double>> InitializeAlgorithm(String team)
    {        
        // TODO create better system for team-NPCType mapping
        String NPCType = "";
        if (team == "TeamA")
            NPCType = "Mushroom";
        else if (team == "TeamB")
            NPCType = "HumanSword";
        
        // Batch ID
        var BatchID = 0;
        var SaveFolder = $"./NEAT/Saves/{NPCType}";
        if (Directory.Exists(SaveFolder))
        {
            BatchID += Directory.GetDirectories(SaveFolder).Length;
        }

        // Experiment ID
        var Id = $"{NPCType}_Batch{BatchID}";
        GD.Print($"Initializing the EA for {Id}...");

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

        // Load latest batch population?
        if (LoadLatestBatch)
        {
            var lastBatchPath = $"{SaveFolder}/Batch_{BatchID - 1}";
            if (Directory.Exists(lastBatchPath))
            {
                try {
                    // Calculate last generation path
                    var lastGen = Directory.GetDirectories(lastBatchPath).Length - 1;
                    var lastGenPath = $"{lastBatchPath}/Gen_{lastGen}";

                    // Create a MetaNeatGenome.
                    var metaNeatGenome = NeatUtils.CreateMetaNeatGenome(experiment);

                    // Load latest genome list
                    var populationLoader = new NeatPopulationLoader<Double>(metaNeatGenome);
                    var lastGenomeList = populationLoader.LoadFromFolder(lastGenPath);

                    // Create Population
                    var lastPopulation = NeatPopulationFactory<double>.CreatePopulation(
                        metaNeatGenome,
                        connectionsProportion: experiment.InitialInterconnectionsProportion,
                        popSize: experiment.PopulationSize
                    );

                    // Recreate new algorithm
                    ea = NeatUtils.CreateNeatEvolutionAlgorithm(experiment, lastPopulation);

                    GD.Print($"Loaded existing population from {lastGenPath}.");
                } catch (IOException e) {
                    GD.Print(e);
                }
            }
        }

        // Update algorithm meta data
        ea.BatchID = BatchID;
        ea.NPCType = NPCType;

        // Initialize the algorithm and run 0th generation
        await ea.Initialise();
        GD.Print($"Initialized the EA for {Id}!");

        return ea;
    }

    private async Task<NeatPopulation<Double>> RunAlgorithm(NeatEvolutionAlgorithm<Double> ea)
    {
        // Evaluate generation
        await ea.PerformOneGeneration();
        var neatPop = ea.Population;
        GD.Print($"({ea.NPCType}) Gen[{ea.Stats.Generation}] Fit_Best={neatPop.Stats.BestFitness.PrimaryFitness}, Fit_Mean={neatPop.Stats.MeanFitness}, Complexity_Mean={neatPop.Stats.MeanComplexity}, Complexity_Mode={ea.ComplexityRegulationMode}");         
        return neatPop;
    }

    private void SavePopulation(NeatEvolutionAlgorithm<Double> ea)
    {
        try
        {
            var folderName = $"{ea.NPCType}/Batch_{ea.BatchID}/Gen_{ea.Stats.Generation}";
            // ea.Population.BestGenome
            NeatPopulationSaver.SaveToFolder(
                ea.Population.GenomeList,
                "./NEAT/Saves/",
                folderName
            );
        } catch (IOException e) {
            GD.Print(e);
        }
    }
    
}

