using UnityEngine;
using System;

namespace SpaceGame.Controllers
{
    public class TargetSelectionSystem : MonoBehaviour
    {
        public bool TryFindTarget(out Targetable target)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                target = player.GetComponent<Targetable>();
                return true;
            }

            target = null;
            return false;
        }
    }
}
