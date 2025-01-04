using UnityEngine;

public class Targetable : MonoBehaviour
{
    public Transform lockPoint;
    private Rigidbody rb;
    private bool hasRigidBody = false;
    public bool active = true;

    private void Awake()
    {
        hasRigidBody = TryGetComponent<Rigidbody>(out rb);
    }

    private void OnEnable()
    {
        if (TryGetComponent<Destructible>(out Destructible destructible))
        {
            destructible.onDestruction += Disable;
        }
    }

    private void OnDisable()
    {
        if (TryGetComponent<Destructible>(out Destructible destructible))
        {
            destructible.onDestruction -= Disable;
        }
    }

    private void Disable()
    {
        active = false;
    }

    public (Vector3, Vector3) GetPositionInfo()
    {
        if (hasRigidBody)
        {
            return (lockPoint.position, rb.linearVelocity);
        }
        return (lockPoint.position, Vector3.zero);
    }

    public Vector3 GetPosition()
    {
        return lockPoint.position;
    }

    public Vector3 GetVelocity()
    {
        return hasRigidBody ? rb.linearVelocity : Vector3.zero;
    }
}
