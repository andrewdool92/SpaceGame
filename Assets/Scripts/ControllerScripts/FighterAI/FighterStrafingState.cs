using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterStrafingState : IFighterState
{
    public override void EnterState(FighterAI ship)
    {
        ship.SetForwardThrusters();
    }

    public override void UpdateState(FighterAI ship)
    {
        if (!ship.TargetIsAhead() || ship.GetTargetDistance() < ship.passDistance)
        {
            ship.SwitchState(ship.passing);
            return;
        }

        if (ship.EvasiveActionRequired())
        {
            ship.SwitchState(ship.adjusting);
        }

        ship.Steer();
    }
}
