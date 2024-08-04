using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAdjustingState : IFighterState
{
    public override void EnterState(FighterAI ship)
    {
        ship.CutThrusters();
    }

    public override void UpdateState(FighterAI ship)
    {
        if (ship.EvasiveActionRequired())
        {
            ship.SetReverse();
            return;
        }

        if (!ship.TargetLocked())
        {
            ship.Steer();
            return;
        }

        if (ship.GetTargetDistance() < ship.firingRange)
        {
            ship.SwitchState(ship.strafing);
        }
        else
        {
            ship.SwitchState(ship.closing);
        }
    }
}
