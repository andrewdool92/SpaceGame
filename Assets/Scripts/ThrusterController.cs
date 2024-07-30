using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour
{
    public List<Thruster> mainThrusters;
    public List<Thruster> reverseThrusters;
    public List<Thruster> leftStrafeThrusters;
    public List<Thruster> rightStrafeThrusters;

    public Thruster leftUp;
    public Thruster rightUp;
    public Thruster leftDown;
    public Thruster rightDown;

    public void SetForwardThrusters(bool value)
    {
        foreach(Thruster thruster in mainThrusters)
        {
            thruster.SetThrust(value);
        }
    }

    public void SetReverseThrusters(bool value)
    {
        foreach (Thruster thruster in reverseThrusters)
        {
            thruster.SetThrust(value);
        }
    }

    public void SetStrafe(float value)
    {
        SetLeftStrafe(value > 0);
        SetRightStrafe(value < 0);
    }

    public void SetUpDown(float value)
    {
        SetUpThrusters(value < 0);
        SetDownThrusters(value > 0);
    }

    public void SetRoll(float value)
    {
        SetRollLeft(value > 0);
        SetRollRight(value < 0);
    }

    public void SetLeftStrafe(bool value)
    {
        foreach(Thruster trail in leftStrafeThrusters)
        {
            trail.SetThrust(value);
        }
    }

    public void SetRightStrafe(bool value)
    {
        foreach (Thruster trail in rightStrafeThrusters)
        {
            trail.SetThrust(value);
        }
    }

    public void SetDownThrusters(bool value)
    {
        leftDown.SetThrust(value);
        rightDown.SetThrust(value);
    }

    public void SetUpThrusters(bool value)
    {
        leftUp.SetThrust(value);
        rightUp.SetThrust(value);
    }

    public void SetRollLeft(bool value)
    {
        rightDown.SetThrust(value);
        leftUp.SetThrust(value);
    }

    public void SetRollRight(bool value)
    {
        rightUp.SetThrust(value);
        leftDown.SetThrust(value);
    }

    public void SetSuperBoost(bool value, bool boosting)
    {
        foreach(Thruster thruster in mainThrusters)
        {
            thruster.SetTrail(value);
            thruster.SetThrust(boosting);
        }
    }
}
