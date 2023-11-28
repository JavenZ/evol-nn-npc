using Godot;
using System;

[GlobalClass]
public partial class InputState : Resource
{
    public float MyHealth {set; get;}

    public float EnemyHealth {set; get;}

    public int MyState {set; get;}

    public int EnemyState {set; get;}

    public float DistanceToEnemy {set; get;}

    public float NextXToEnemy {set; get;}

    public float NextYToEnemy {set; get;}

    public override string ToString()
    {
        return $"InputState: MyHealth={MyHealth}, EnemyHealth={EnemyHealth}, MyState={MyState}, EnemyState={EnemyState}, DistanceToEnemy={DistanceToEnemy}, NextX={NextXToEnemy}, NextY={NextYToEnemy}";
    }

}
