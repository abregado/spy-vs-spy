using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour {
    private TextMeshProUGUI _timeDisplay;
    
    public void Awake() {
        _timeDisplay = GetComponent<TextMeshProUGUI>();
    }

    public void DisplayTime(float time) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeText = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        _timeDisplay.text = timeText;
    }
}
