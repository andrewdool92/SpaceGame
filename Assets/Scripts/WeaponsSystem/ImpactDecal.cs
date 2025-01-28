using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ImpactDecal : MonoBehaviour
{
    public DecalProjector outer;
    public DecalProjector inner;

    public float fadeSpeed = 1f;
    public float fadeValue = 0f;

    private bool fading = false;

    // Async caused errors on application closed; switched to coroutines, which die with the gameobject
    private async void FadeAsync()
    {
        SetFadeValue(1f);

        if (!fading)
        {
            fading = true;

            while (fadeValue > 0)
            {
                await Task.Delay(100);
                SetFadeValue(fadeValue - (fadeSpeed * .1f));
            }

            fading = false;
        }
    }

    private IEnumerator Fade()
    {
        SetFadeValue(1f);

        if (!fading)
        {
            fading = true;

            while (fadeValue > 0)
            {
                yield return new WaitForSeconds(.1f);
                SetFadeValue(fadeValue - (fadeSpeed * .1f));
            }

            fading = false;
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
    }

    public void Apply(float size, RaycastHit hit)
    {
        if (!hit.transform.gameObject.activeInHierarchy)
        {
            return;
        }

        outer.size = new Vector3(size, size, 2);
        inner.size = new Vector3(size / 5, size / 5, 2);

        transform.position = hit.point + hit.normal * .5f;
        transform.transform.forward = -hit.normal;
        transform.parent = hit.transform;
        StartCoroutine(Fade());
    }
}
