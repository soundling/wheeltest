using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Wheel wheel;

    public TextMeshProUGUI labelResult;

    public Button nextButton;
    public Button spinButton;
    void Start()
    {
        wheel.onWheelSpinEnd += OnWheelEndResult;
        nextButton.onClick.AddListener((() =>
        {
            wheel.NextWheel();
        }));
        spinButton.onClick.AddListener((() =>
        {
            wheel.TrySpin();
        }));
    }

    public void OnWheelEndResult(WheelResultData d)
    {
        labelResult.text = d.isJackpot ? $"JACKPOT - You won {d.euros} Euros" : $"Congratz! You won {d.euros} Euros";
    }

    public void OnDestroy()
    {
        wheel.onWheelSpinEnd -= OnWheelEndResult;
    }
}
