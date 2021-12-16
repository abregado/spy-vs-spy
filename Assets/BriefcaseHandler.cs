using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BriefcaseHandler : MonoBehaviour {
    public G.ItemType[] slotConfig;
    public bool[] slotStates;

    private bool _completeAndUnlocked;

    private BriefcaseUI _briefcaseUI;

    public void Init() {
        slotStates = new bool[slotConfig.Length];
        _briefcaseUI = FindObjectOfType<BriefcaseUI>();
        _briefcaseUI.Init();
        }

    public bool CheckNeedsItem(G.ItemType item) {
        for (int i = 0; i < slotStates.Length; i++) {
            if (slotConfig[i] == item && slotStates[i] == false) {
                return true;
            }
        }

        return false;
    }

    public bool PutInItem(G.ItemType item) {
        for (int i = 0; i < slotStates.Length; i++) {
            if (slotConfig[i] == item && slotStates[i] == false) {
                slotStates[i] = true;
                _briefcaseUI.UpdateImageVisiblity(i,true);
                return true;
            }
        }

        if (IsComplete() && _completeAndUnlocked == false) {
            SetOneExitUnlocked();
        }
        
        return false;
    }

    public void SetOneExitUnlocked() {
        
        ExitDoor[] exitDoors = FindObjectsOfType<ExitDoor>();
        
        exitDoors[Random.Range(0,exitDoors.Length)].SetUnlocked();
        _completeAndUnlocked = true;
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
