using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using StarterAssets;
using UnityEngine.Events;
public class GameManager : MonoBehaviour
{
    bool isInside;
    public List<Health> enemies;

    public UnityEvent onStart;
    public UnityEvent onFinish;
    bool finished = false;

    float timeInterval = .33f;
    public TextCountDownAnimation countDown;

    private void Start()
    {
        enemies = GetComponentsInChildren<Health>().ToList<Health>();
        Time.timeScale = 0f;
        StartCoroutine(GameInit());
    }

    IEnumerator GameInit()
    {
        yield return new WaitForSecondsRealtime(2);
        for (int i =3; i>=0; i--)
        {
            Debug.Log("counting down: " + i);
            string s;
            if (i != 0) s = i.ToString();
            else s = "GO";
            countDown.ShowText(s, timeInterval);
            yield return new WaitForSecondsRealtime(timeInterval);
        }
        Time.timeScale = 1f;
        countDown.gameObject.SetActive(false);
        onStart.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!finished && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (CheckKills()) Finish();
            else isInside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isInside = false;
        }
    }
    void Finish()
    {
        InputManager.Instance.SetCursorState(false);
        onFinish.Invoke();
        finished = true;
        Time.timeScale = 0f;
    }

    private bool CheckKills()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (!enemies[i].dead) return false;
        }
        return true;
    }

    private void Update()   
    {
        if (!finished && isInside && CheckKills()) Finish();
    }
}
