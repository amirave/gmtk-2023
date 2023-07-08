using System.Collections;
using System.Collections.Generic;
using Core;
using Screenshake;
using Timers;
using UnityEngine;

public class ScreenShakeTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TimerService.Instance.RegisterTimer(1, Shake, true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Shake()
    {
        ScreenShakeService.Instance.AddScreenShake(0.5f);
    }
}
