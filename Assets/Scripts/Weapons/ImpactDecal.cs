using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ImpactDecal : MonoBehaviour
{
    public DecalProjector outer;
    public DecalProjector inner;

    public float fadeSpeed = 1f;
    public float fadeValue = 0f;

    private IEnumerator Fade()
    {
        while (true)
        {
            if (fadeValue > 0)
            {
                fadeValue -= fadeSpeed * .1f;
                outer.fadeFactor = fadeValue;
                inner.fadeFactor = fadeValue;
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public void SetFadeValue(float value)
    {
        fadeValue = value;
        outer.fadeFactor = fadeValue;
        inner.fadeFactor = fadeValue;
    }

    public void Initialize()
    {
        SetFadeValue(0f);
        StartCoroutine(Fade());
    }

    public void Apply(float size, RaycastHit hit)
    {
        outer.size = new Vector3(size, size, 2);
        inner.size = new Vector3(size / 5, size / 5, 2);

        transform.position = hit.point + hit.normal * .5f;
        transform.transform.forward = -hit.normal;
        transform.parent = hit.transform;
        SetFadeValue(1f);
    }
}
