using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class MouseClickCatcher : MonoBehaviour {
    private RoomCameraSystem _cameraSystem;
    private SpyHandler _spyHandler;
    private int _interactionLayerMask = 1 << 3;
    
    void Awake() {
        _cameraSystem = FindObjectOfType<RoomCameraSystem>();
        _spyHandler = FindObjectOfType<SpyHandler>();
    }
    
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePos = Input.mousePosition;
            // Debug.Log("mouse position " + mousePos);
            // Debug.Log(mousePos);
            
            for (int p = 0; p < 4; p++) {
                Rect playerCameraRect = _cameraSystem.playerCameras[p].rect;
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                Rect inputRect = new Rect(playerCameraRect.position * screenSize, playerCameraRect.size * screenSize);
                if (inputRect.Contains(mousePos)) {
                    ActivateRayTarget(p);
                }
            }
        }
    }

    private void ActivateRayTarget(int playerIndex) {
        
        Camera playerCamera = _cameraSystem.GetPlayerCamera(playerIndex);
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        GameObject hitMarker = FindObjectOfType<HitMarker>().gameObject;
        
        if (Physics.Raycast(ray, out hit, 5000f,_interactionLayerMask)) {
            hitMarker.transform.position = hit.point;    
            IInteractable interactable = hit.collider.transform.GetComponent<IInteractable>();
            if (interactable != null) {
                interactable.OnInteract(_spyHandler.spies[playerIndex]);
            }
        }
    }
    
}
