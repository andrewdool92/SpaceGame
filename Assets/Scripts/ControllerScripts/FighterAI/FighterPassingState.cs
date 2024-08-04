using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterPassingState : IFighterState
{
    private float boostDelay;
    private Vector3 aimPoint;

    public override void EnterState(FighterAI ship)
    {
        boostDelay = 1.5f;
        ship.CutThrusters();

        //ship.SetSuperBoost(true);
        aimPoint = ship.target.position + ship.transform.forward * ship.passDistance * 5 + ship.transform.up * ship.passDistance * 5;
        //Debug.DrawLine(ship.transform.position, aimPoint, Color.yellow, 2f);
    }

    public override void UpdateState(FighterAI ship)
    {
        if (boostDelay > 0) boostDelay -= Time.deltaTime;
        else ship.SetSuperBoost(true);

        if (ship.EvasiveActionRequired())
        {
            ship.SwitchState(ship.adjusting);
        }

        else if (ship.TargetIsAhead() && ship.GetTargetDistance() > ship.passDistance + 20)
        {
            ship.SetSuperBoost(false);
            ship.SwitchState(ship.strafing);
        }

        else if (ship.GetTargetDistance() > ship.retreatDistance)
        {
            ship.SetSuperBoost(false);
            ship.SwitchState(ship.adjusting);
        }

        else
        {
            ship.Steer(aimPoint);
        }
    }
}
