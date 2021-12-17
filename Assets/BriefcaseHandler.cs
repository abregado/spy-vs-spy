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
        _completeAndUnlocked = false;
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
                
                if (IsComplete() && _completeAndUnlocked == false) {
                    SetOneExitUnlocked();
                }

                return true;
            }
        }

        
        
        return false;
    }

    public void SetOneExitUnlocked() {
        Debug.Log("The way is open");
        
        LevelGenerator genny = FindObjectOfType<LevelGenerator>();
        genny.exitDoors[Random.Range(0,genny.exitDoors.Count)].SetUnlocked();
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
