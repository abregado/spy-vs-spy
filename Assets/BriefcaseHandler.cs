using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseHandler : MonoBehaviour {
    public MeshRegistry.ItemType[] slotConfig;
    public bool[] slotStates;

    private BriefcaseUI _briefcaseUI;

    void Awake() {
        slotStates = new bool[slotConfig.Length];
        _briefcaseUI = FindObjectOfType<BriefcaseUI>();
    }

    private void Start() {
        // for (int i = 0; i < slotStates.Length; i++) {
        //     _briefcaseUI.UpdateImageVisiblity(i,false);
        // }
    }

    public bool CheckNeedsItem(MeshRegistry.ItemType item) {
        for (int i = 0; i < slotStates.Length; i++) {
            if (slotConfig[i] == item && slotStates[i] == false) {
                return true;
            }
        }

        return false;
    }

    public bool PutInItem(MeshRegistry.ItemType item) {
        for (int i = 0; i < slotStates.Length; i++) {
            if (slotConfig[i] == item && slotStates[i] == false) {
                slotStates[i] = true;
                _briefcaseUI.UpdateImageVisiblity(i,true);
                return true;
            }
        }

        return false;
    }

    public bool IsComplete() {
        foreach (bool state in slotStates) {
            if (state == false) {
                return false;
            }
        }

        return true;
    }
    
}
