using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlasterController : MonoBehaviour
{
    public List<Transform> weaponHardpoints;
    public List<Animator> weaponAnimations;
    public ParticleSystem muzzleFlare;

    public Blaster blaster;
    private Queue<Projectile> projectilePool;
    public int maxProjectiles = 20;

    private int blasterIndex = 0;

    public bool firing = false;

    public float firingSpeed;
    public float timer;

    private Rigidbody rb;

    private bool animate = false;
    private bool flareEffect = false;
    private int animationTrigger = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitializeWeapons();

        timer = 0;

        if (weaponAnimations.Count == weaponHardpoints.Count)
        {
            animate = true;
            animationTrigger = Animator.StringToHash("Fire");
        }

        if (muzzleFlare) flareEffect = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (firing && timer <= 0)
        {
            timer += firingSpeed;
            SingleShot();
        }
    }

    private void SingleShot()
    {
        blaster.Fire(weaponHardpoints[blasterIndex], transform.forward, rb.velocity);
        PlayMuzzleFlare(blasterIndex);

        if (animate) weaponAnimations[blasterIndex].SetTrigger(animationTrigger);

        blasterIndex = (blasterIndex + 1) % weaponHardpoints.Count;
    }

    public void HandleFireInput(InputAction.CallbackContext context)
    {
        firing = context.performed;
    }

    public void FireBurst(int rounds)
    {
        StartCoroutine(FireRounds(rounds));
    }

    private IEnumerator FireRounds(int rounds)
    {
        int fired = 0;
        while (fired < rounds)
        {
            SingleShot();
            fired++;
            yield return new WaitForSeconds(firingSpeed);
        }
    }

    private void InitializeWeapons()
    {
        projectilePool = blaster.GenerateProjectilPool();
    }

    public Vector3 CalculateLeadPoint(Vector3 firingPoint, Vector3 target, Vector3 targetVelocity)
    {
        return CalculateLeadPoint(firingPoint, target, targetVelocity, Mathf.Infinity);
    }

    public Vector3 CalculateLeadPoint(Vector3 firingPoint, Vector3 target, Vector3 targetVelocity, float maxCompensation)
    {
        float distance = (target - firingPoint).magnitude;
        float timeToHit = distance / blaster.projectileSpeed;

        return target + targetVelocity * Mathf.Min(timeToHit, maxCompensation);
    }

    private void PlayMuzzleFlare(int index)
    {
        if (flareEffect)
        {
            muzzleFlare.transform.position = weaponHardpoints[index].position;
            muzzleFlare.transform.rotation = transform.rotation;
            muzzleFlare.Play();
        }
    }
}
