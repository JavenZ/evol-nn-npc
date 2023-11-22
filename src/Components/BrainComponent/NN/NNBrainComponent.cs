using Godot;
using System;
using SharpNeat;

[GlobalClass]
public partial class NNBrainComponent : Node
{
    IBlackBox<double> Box {set; get;}

    public OutputDecision nextMove(InputState input)
    {
        var output = new OutputDecision()
        {
            x=0,
            y=0,
            jump=false,
            attack=false,
        };
        return output;
    }
}
