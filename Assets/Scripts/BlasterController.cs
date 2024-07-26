using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlasterController : MonoBehaviour
{
    public List<ParticleSystem> blasters;
    private int blasterIndex = 0;

    public bool firing = false;

    public float firingSpeed;
    public float timer;

    private void Start()
    {
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
            blasters[blasterIndex].Play();
            blasterIndex = (blasterIndex + 1) % blasters.Count;
        }
    }

    public void HandleFireInput(InputAction.CallbackContext context)
    {
        firing = context.performed;
    }
}
