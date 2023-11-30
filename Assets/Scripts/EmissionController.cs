using UnityEngine;
using System.Collections;

public class EmissionController : MonoBehaviour
{
    public Color colorA;
    public Color colorB;

    public float intensityA;
    public float intensityB;

    private Material material;
    private Coroutine changing;

    void Start()
    {
        // Get the SkinnedMeshRenderer's material
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        material = skinnedMeshRenderer.material;
    }

    // Function to change emission from min to max over a specified time
    public void ChangeColorAtoB(float timeToReach)
    {
        if (changing != null) StopCoroutine(changing);
        changing = StartCoroutine(ChangeColor(colorA*intensityA, colorB*intensityB, timeToReach));
    }

    // Function to change emission from max to min over a specified time
    public void ChangeColorBtoA(float timeToReach)
    {
        if (changing != null) StopCoroutine(changing);
        changing = StartCoroutine(ChangeColor(colorB* intensityB, colorA* intensityA, timeToReach));
    }
    public void ChangeCurrentColortoA()
    {
        if (changing != null) StopCoroutine(changing);
        Color currentColor = material.GetColor("_EmissionColor");
        changing = StartCoroutine(ChangeColor(currentColor, colorA * intensityA, .1f));
    }

    private IEnumerator ChangeColor(Color startColor, Color endColor, float timeToReach)
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeToReach)
        {
            float t = elapsedTime / timeToReach;
            Color newEmissionColor = Color.Lerp(startColor, endColor, t);

            material.SetColor("_EmissionColor", newEmissionColor);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Ensure the final value is set
        material.SetColor("_EmissionColor", endColor);
    }
}