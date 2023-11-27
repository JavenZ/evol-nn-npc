using Godot;
using Godot.Collections;
using SharpNeat;

// [GlobalClass]
public partial class GamePool
{
    private static Godot.Mutex mutex = new Godot.Mutex();
    private static int game_index = 0;
    private static Array<Node2D> pool;
    private static Godot.Collections.Dictionary<Node2D, Array<NNBrainComponent>> BrainPool;

    public Trainer trainer {set; get;}

    public void Initialize(int size)
    {
        // NOT THREAD SAFE
        GD.Print("Initializing game pool...");
        game_index = 0;
        pool = new Array<Node2D>();
        BrainPool = new Godot.Collections.Dictionary<Node2D, Array<NNBrainComponent>>();

        PackedScene game_scene = GD.Load("res://Game/Game.tscn") as PackedScene;
        PackedScene human_scene = GD.Load("res://NPCs/Human_Sword/Human_Sword.tscn") as PackedScene;
        PackedScene mushroom_scene = GD.Load("res://NPCs/Mushroom/Mushroom.tscn") as PackedScene;
        PackedScene brain_scene = GD.Load("res://Components/BrainComponent/NN/NNBrainComponent.tscn") as PackedScene;
        
        int j = 0;
        for (int i = 0; i < size; i++)
        {
            // Instantiate game node
            var game = game_scene.Instantiate() as Node2D;
            BrainPool[game] = new Array<NNBrainComponent>();
            
            // Space out game position
            if (i % 10 == 0) j++;
            var new_pos = game.GlobalPosition;
            new_pos.X += 1100 * (i % 10);
            new_pos.Y += 520 * j;
            game.Position = new_pos;

            // Instantiate game teams
            var team_a = new Array<Node>();
            var mush1 = mushroom_scene.Instantiate();
            var brain1 = brain_scene.Instantiate() as NNBrainComponent;
            BrainPool[game].Add(brain1);
            mush1.Set("brain_component", brain1);
            team_a.Add(mush1);
            game.Set("team_a", team_a);

            var team_b = new Array<Node>();
            var human1 = human_scene.Instantiate();
            var brain2 = brain_scene.Instantiate() as NNBrainComponent;
            BrainPool[game].Add(brain2);
            human1.Set("brain_component", brain2);
            team_b.Add(human1);
            game.Set("team_b", team_b);

            // Add game to pool
            pool.Add(game);
        }
        GD.Print("Game pool initialized!");
    }

    public async Task<float> StartGame(IBlackBox<double> box)
    {
        // THREAD SAFE
        GD.Print("Starting game...");

        // Select available game from pool
        mutex.Lock();
        var game = pool[game_index];
        game_index += 1;
        mutex.Unlock();

        // Update brain component with box
        foreach (NNBrainComponent brain in BrainPool[game])
        {
            brain.Box = box;
        }

        // Add game to scene tree
        trainer.CallDeferred("add_child", game);

        // Wait for game to finish
        var results = await game.ToSignal(game, "finished");
        GD.Print("Game finished!");

        // TODO Return game results
        return 0.0F;
    }


}