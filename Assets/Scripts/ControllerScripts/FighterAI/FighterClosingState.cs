using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterClosingState : IFighterState
{
    public override void EnterState(FighterAI ship)
    {
        ship.SetSuperBoost(true);
    }

    public override void UpdateState(FighterAI ship)
    {
        if (ship.TargetLocked() && ship.GetTargetDistance() < ship.firingRange)
        {
            ship.SetSuperBoost(false);
            ship.SwitchState(ship.strafing);
            return;
        }
        else if (!ship.TargetIsAhead())
        {
            ship.SetSuperBoost(false);
            ship.SwitchState(ship.adjusting);
            return;
        }
        ship.Steer();
    }
}
