using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [Range(1, 100)]
    public int fontSize;

    [Range(0, 1)]
    public float Red, Green, Blue;

    public TextMeshProUGUI textFPS;

    float deltaTime = 0f;
    StringBuilder strFPS = new StringBuilder();
    // Start is called before the first frame update
    void Start()
    {
        fontSize = fontSize == 0 ? 50 : fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        ShowFPS();
    }

    void ShowFPS()
    {
        float msec = deltaTime * 1000f;
        float fps = 1.0f / deltaTime;
        strFPS.Clear();
        strFPS.Append(string.Format("{0:0.0}ms ({1:0.}fps)", msec, fps));
        textFPS.text = strFPS.ToString();
    }
}
