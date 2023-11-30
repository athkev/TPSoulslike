using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthandSpecialBar : MonoBehaviour
{
    public Slider hpSlider;
    public Slider specialSlider;
    private void Update()
    {
        UpdateHP();
        UpdateSP();
    }
    void UpdateHP()
    {
        hpSlider.value = PlayerHealth.Instance.currentHP/PlayerHealth.Instance.initialHP;
    }
    void UpdateSP()
    {
        specialSlider.value = DefenseStance.Instance.currentSpecialCounter;
    }
}
