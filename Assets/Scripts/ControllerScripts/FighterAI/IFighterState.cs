using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFighterState
{
    public abstract void EnterState(FighterAI ship);
    public abstract void UpdateState(FighterAI ship);
}
