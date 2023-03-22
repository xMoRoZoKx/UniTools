using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
 
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private float time = 0f;
    private int i = 0;
    void Update()
    {
        i++;
        time += Time.deltaTime;
        if(time>= 1.0f)
        {
            time = 0;
            text.text = "FPS: " + i.ToString(".0");
            i = 0;
        }
    }
}
