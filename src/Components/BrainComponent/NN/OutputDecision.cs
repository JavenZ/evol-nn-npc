using Godot;
using System;

[GlobalClass]
public partial class OutputDecision : Resource
{
    public int x {set; get;}
    public bool jump {set; get;}
    public bool attack {set; get;}

    public override string ToString()
    {
        return $"OutputDecision: x={x}, jump={jump}, attack={attack}";
    }
}
