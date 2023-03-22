using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private float timeFromLastUpdate = 0f;
    private int fpsCount = 0;
    void Update()
    {
        fpsCount++;
        timeFromLastUpdate += Time.deltaTime;
        if (timeFromLastUpdate >= 1)
        {
            text.text = "FPS: " + fpsCount;
            timeFromLastUpdate = 0;
            fpsCount = 0;
        }
    }
}
