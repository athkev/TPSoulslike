using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextCountDownAnimation : MonoBehaviour
{
    TextMeshProUGUI countdownText;
    public float maxScale = 7f;
    public float minScale = 2f;
    Coroutine scaleAnim;

    private void Start()
    {
        countdownText = GetComponent<TextMeshProUGUI>();
    }
    public void ShowText(string text, float time)
    {
        countdownText.enabled = true;
        if (scaleAnim != null) StopCoroutine(scaleAnim);
        countdownText.text = text;
        scaleAnim = StartCoroutine(ChangeSize(time));
    }

    IEnumerator ChangeSize(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            float t = elapsedTime / time;

            float newScale = Mathf.Lerp(maxScale, minScale, t);

            transform.localScale = new Vector3(newScale, newScale, newScale);
            elapsedTime += Time.unscaledDeltaTime;

            // Wait for the next frame
            yield return null;
        }

        Vector3 finalScale = new Vector3(minScale, minScale, minScale);
        transform.localScale = finalScale;
    }


}
