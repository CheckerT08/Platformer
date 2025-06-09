using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSText : MonoBehaviour
{
    TextMeshProUGUI text;
    float time;

    void Start()
    {
        Application.targetFrameRate = 60;
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 0.3)
        {
            time = 0f;
            Refresh();
        }
    }

    void Refresh()
    {
        text.text = (1 / Time.deltaTime).ToString() + " FPS";
    }
}
