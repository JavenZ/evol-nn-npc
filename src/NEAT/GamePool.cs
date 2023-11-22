using Godot;
using Godot.Collections;

// [GlobalClass]
public partial class GamePool
{
    private static Godot.Mutex mutex = new Godot.Mutex();
    private static int game_index = 0;
    private static Array<Node2D> pool;

    public Trainer trainer {set; get;}

    public void Initialize(int size)
    {
        // NOT THREAD SAFE
        GD.Print("Initializing game pool...");
        game_index = 0;
        pool = new Array<Node2D>();

        PackedScene game_scene = GD.Load("res://Game/Game.tscn") as PackedScene;
        PackedScene human_scene = GD.Load("res://NPCs/Human_Sword/Human_Sword.tscn") as PackedScene;
        PackedScene mushroom_scene = GD.Load("res://NPCs/Mushroom/Mushroom.tscn") as PackedScene;
        
        for (int i = 0; i < size; i++)
        {
            // Instantiate game node
            var game = game_scene.Instantiate() as Node2D;
            
            // Space out game position
            var new_pos = game.GlobalPosition;
            new_pos.X += 1100 * i;
            game.Position = new_pos;

            // Instantiate game teams
            var team_a = new Array<Node>();
            var mush1 = mushroom_scene.Instantiate();
            // TODO perform brain init?
            team_a.Add(mush1);
            game.Set("team_a", team_a);

            var team_b = new Array<Node>();
            var human1 = human_scene.Instantiate();
            // TODO perform brain init?
            team_b.Add(human1);
            game.Set("team_b", team_b);

            // Add game to pool
            pool.Add(game);
        }
        GD.Print("Game pool initialized!");
    }

    public async Task<float> StartGame()
    {
        // THREAD SAFE
        GD.Print("Starting game...");

        // Select available game from pool
        mutex.Lock();
        var game = pool[game_index];
        game_index += 1;
        mutex.Unlock();

        // Add game to scene tree
        trainer.CallDeferred("add_child", game);

        // Wait for game to finish
        var results = await game.ToSignal(game, "finished");
        GD.Print("Game finished!");

        // TODO Return game results
        return 0.0F;
    }


}