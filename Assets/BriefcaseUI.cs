using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BriefcaseUI : MonoBehaviour {
    public Image[] itemImage;

    private bool _visible;
    
    private void Awake() {
        
    }

    public void UpdateImageVisiblity(int imageIndex,bool state) {
        itemImage[imageIndex].enabled = state;
        if (state) {
            _visible = true;
        }
        SetPanelVisiblity(_visible);
    }

    private void SetPanelVisiblity(bool state) {
        GetComponent<Image>().enabled = state;
    }
}