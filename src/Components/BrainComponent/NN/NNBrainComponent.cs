using Godot;
using System;
using SharpNeat;

#pragma warning disable

[GlobalClass]
public partial class NNBrainComponent : Node
{
    public IBlackBox<double> Box {set; get;}

    public override void _Ready()
    {
        // GD.Print($"Brain box: {Box}");
    }

    public OutputDecision NextMove(InputState input)
    {
        if (Box != null)
        {
            return Activate(input);
        }
        else
        {
            return new OutputDecision()
            {
                x=0,
                jump=false,
                attack=false,
            };
        }
    }

    #region Private Static Methods

    private OutputDecision Activate(InputState state)
    {
        var inputs = Box.Inputs.Span;
        var outputs = Box.Outputs.Span;
        var decision = new OutputDecision();

        // Bias input
        inputs[0] = 1.0;

        // Read input state
        inputs[1] = state.MyHealth;
        inputs[2] = state.EnemyHealth;
        inputs[3] = state.MyState;
        inputs[4] = state.EnemyState;
        inputs[5] = state.DistanceToEnemy;
        inputs[6] = state.NextXToEnemy;
        inputs[7] = state.NextYToEnemy;

        // Activate the black box
        Box.Activate();

        // Read output signals
        decision.x = 0;
        if (outputs[0] >= 0.50)
            decision.x = 1;
        if (outputs[0] < 0.0)
            decision.x = -1;
            
        decision.jump = false;
        if (outputs[1] >= 0.50)
            decision.jump = true;
        
        decision.attack = false;
        if (outputs[2] >= 0.50)
            decision.attack = true;
        
        // GD.Print($"{state}\n{decision}");

        return decision;
    }

    #endregion
}
