using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlasterController : MonoBehaviour
{
    public List<Transform> weaponHardpoints;
    public Blaster blaster;
    private Queue<Projectile> projectilePool;
    public int maxProjectiles = 20;

    private int blasterIndex = 0;

    public bool firing = false;

    public float firingSpeed;
    public float timer;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InitializeWeapons();

        timer = 0;
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
}
