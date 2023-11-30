using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using StarterAssets;
public class DeathHandler : MonoBehaviour
{
    public float waitTimer = 1f;
    public GameObject deathCanvas;

    PlayerHealth player;

    private void Start()
    {
        deathCanvas.SetActive(false);
        player = PlayerHealth.Instance;
        if (player != null) player.onDeath.AddListener(ShowDeathCanvas);
    }
    public void ShowDeathCanvas()
    {
        InputManager.Instance.SetCursorState(false);
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(waitTimer);
        deathCanvas.SetActive(true);
    }
}
