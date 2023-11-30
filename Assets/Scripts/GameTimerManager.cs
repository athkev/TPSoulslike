using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimerManager : MonoBehaviour
{
    public static GameTimerManager Instance
    {
        get
        {
            if (_instance == null) { Debug.Log("no time controller"); return null; }
            return _instance;
        }
    }
    private static GameTimerManager _instance;

    private float elapsedTime;
    private bool stoped = true;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    private void Start()
    {
        _instance = this;
    }

    public void StartGameTimer()
    {
        stoped = false;
    }

    public void StopGameTimer()
    {
        stoped = true;
    }
    public string GetTimer()
    {
        float minutes = Mathf.FloorToInt(elapsedTime / 60);
        float seconds = Mathf.FloorToInt(elapsedTime % 60);
        float milliseconds = (elapsedTime % 1) * 1000;
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    private void Update()
    {
        if (!stoped)
        {
            elapsedTime += Time.deltaTime;
            UpdateDisplay();
        }
    }
    private void UpdateDisplay()
    {
        timerText.text = GetTimer();
        resultText.text = GetTimer();
    }
}
