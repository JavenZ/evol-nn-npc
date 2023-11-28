using Godot;
using Godot.Collections;
using SharpNeat;

// [GlobalClass]
public partial class GamePool
{
    private static Godot.Mutex Mutex = new Godot.Mutex();
    private static List<GameSession> Pool;

    public Trainer Trainer {set; get;}

    public int Size {set; get;}

    public void Initialize()
    {
        // NOT THREAD SAFE
        // GD.Print("Initializing game pool...");
        Pool = new List<GameSession>(Size);

        PackedScene game_scene = GD.Load("res://Game/Game.tscn") as PackedScene;
        PackedScene human_scene = GD.Load("res://NPCs/Human_Sword/Human_Sword.tscn") as PackedScene;
        PackedScene mushroom_scene = GD.Load("res://NPCs/Mushroom/Mushroom.tscn") as PackedScene;
        PackedScene brain_scene = GD.Load("res://Components/BrainComponent/NN/NNBrainComponent.tscn") as PackedScene;
        
        int j = 0;
        for (int i = 0; i < Size; i++)
        {
            // Instantiate game session
            var session = new GameSession();
            session.ID = i;

            // Instantiate game node
            session.Game = game_scene.Instantiate() as Node2D;
            session.Game.Set("name", $"game_{i}");
            
            // Space out game position
            if (i % 10 == 0) j++;
            var new_pos = session.Game.GlobalPosition;
            new_pos.X += 1100 * (i % 10);
            new_pos.Y += 520 * j;
            session.Game.Position = new_pos;

            // Instantiate game teams
            session.TeamABrain = brain_scene.Instantiate() as NNBrainComponent;
            var mushroom = mushroom_scene.Instantiate();
            mushroom.Set("brain_component", session.TeamABrain);
            session.TeamA.Add(mushroom);
            session.Game.Set("team_a", session.TeamA);

            session.TeamBBrain = brain_scene.Instantiate() as NNBrainComponent;
            var human = human_scene.Instantiate();
            human.Set("brain_component", session.TeamBBrain);
            session.TeamB.Add(human);
            session.Game.Set("team_b", session.TeamB);

            // Add game session to pool
            Pool.Add(session);
        }
        // GD.Print("Game pool initialized!");
    }

    public void Reset()
    {
        // NOT THREAD SAFE
        GD.Print("Resetting game pool...");

        // Remove all games from scene tree
        foreach (GameSession session in Pool)
        {
            Trainer.RemoveChild(session.Game);
        }

        // Re-initialize game pool
        Initialize();
    }

    public async Task<GameResults> JoinGame(IBlackBox<double> box, String team)
    {
        // THREAD SAFE
        // GD.Print($"{team} joining game...");
        GameSession session;

        // Select first available game from pool & update respective brain components with box
        Mutex.Lock();
        if (team == "TeamA")
        {
            session = Pool.First(session => !session.TeamAReady);
            session.TeamABrain.Box = box;
            session.TeamAReady = true;
            // GD.Print($"TeamA joined session {session.ID}");
        }
        else if (team == "TeamB")
        {
            session = Pool.First(session => !session.TeamBReady);
            session.TeamBBrain.Box = box;
            session.TeamBReady = true;
            // GD.Print($"TeamB joined session {session.ID}");
        }
        else
        {
            session = Pool.First(session => !session.TeamAReady && !session.TeamBReady);
            session.TeamABrain.Box = box;
            session.TeamBBrain.Box = box;
            session.TeamAReady = true;
            session.TeamBReady = true;
            // GD.Print($"TeamA & TeamB joined session {session.ID}");
        }

        // Start game session if both teams are ready
        if (session.TeamAReady && session.TeamBReady)
        {
            Trainer.CallDeferred("add_child", session.Game);
            // GD.Print($"Started session {session.ID}!");
        }

        Mutex.Unlock();

        // Wait for game session to finish
        var results = await session.Game.ToSignal(session.Game, "finished");
        // GD.Print($"Session {session.ID} finished!");

        // Return game results
        return results[0].As<GameResults>();
    }
    private class GameSession {
        public int ID {set; get;}
        public Node2D Game {set; get;}

        public Array<Node> TeamA = new Array<Node>();
        public Array<Node> TeamB = new Array<Node>();

        public NNBrainComponent TeamABrain {set; get;}
        public NNBrainComponent TeamBBrain {set; get;}

        public bool TeamAReady {set; get;}
        public bool TeamBReady {set; get;}
    }

}