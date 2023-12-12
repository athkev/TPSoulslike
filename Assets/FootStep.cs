using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : RandomSFX
{
    ThirdPersonController controller;
    public float timeThreshold = .1f;
    float currentThreshold = 0f;
    private void Start()
    {
        controller = GetComponent<ThirdPersonController>();
    }
    public void PlayFootStep()
    {
        if (controller.enableMovement && (controller.Grounded ||  controller.isWallRunning) && currentThreshold > timeThreshold)
        {
            PlayRandom();
            currentThreshold = 0f;
        }   
    }
    private void Update()
    {
        if (currentThreshold < timeThreshold)
        {
            currentThreshold += Time.deltaTime;
        }
    }
}
