using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
    public int roomTarget;
    private RoomCameraSystem _cameraSystem;

    public bool interactable => true;

    void Awake() {
        _cameraSystem = FindObjectOfType<RoomCameraSystem>();
    }
    
    public void OnInteract(Spy spy) {
        _cameraSystem.SwitchCameraToRoom(spy.playerIndex,roomTarget);
        spy.ChangeRoom(_cameraSystem.editorRooms[roomTarget]);
    }
}

