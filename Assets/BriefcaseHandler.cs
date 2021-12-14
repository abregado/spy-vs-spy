using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefcaseHandler : MonoBehaviour {
    public MeshRegistry.ItemType[] slotConfig;
    public bool[] slotStates;

    void Awake() {
        slotStates = new bool[slotConfig.Length];
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
                return true;
            }
        }

        return false;
    }
    
}
