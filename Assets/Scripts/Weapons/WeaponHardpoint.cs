using UnityEngine;

public class WeaponHardpoint : MonoBehaviour
{
    public Animator animator;
    private bool animated = false;
    private int animatorTrigger = 0;

    private void Start()
    {
        if (animator != null)
        {
            animated = true;
            animatorTrigger = Animator.StringToHash("Fire");
        }
    }

    public void PlayAnimation()
    {
        if (animated)
        {
            animator.SetTrigger(animatorTrigger);
        }
    }
}
