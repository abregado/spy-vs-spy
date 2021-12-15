using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
    public Door doorTarget;
    public Room myRoom;

    public bool interactable => true;

    void Awake() {
        myRoom = transform.GetComponentInParent<Room>();
    }
    
    public void OnInteract(Spy spy) {
        spy.GoToDoor(doorTarget);
    }

    public Vector3 GetExitPosition() {
        return transform.Find("ExitPos").transform.position;
    }
}

