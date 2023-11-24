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

        // Activate the black box
        Box.Activate();

        // GD.Print($"x={outputs[0]}, jump={outputs[1]}, atk={outputs[2]}");

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

        return decision;
    }

    #endregion
}
