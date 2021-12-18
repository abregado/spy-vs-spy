using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Room : MonoBehaviour {
    public bool canSpawnHere = true;

    private float nextMapUpdate;

    private GameObject _itemIndicator;
    private CinemachineVirtualCamera[] _cameras;
    public RoomTile myTile;
    public Dictionary<G.GridDir, Door> doors = new Dictionary<G.GridDir, Door>();
    public List<Furniture> furniture = new List<Furniture>();

    
    // Start is called before the first frame update
    void Awake() {
        _cameras = transform.GetComponentsInChildren<CinemachineVirtualCamera>();
        _itemIndicator = transform.Find("ItemIndicator").gameObject;
    }

    public void InitDoorIndicators() {
        transform.Find("DoorIndicators/Top").gameObject.SetActive(doors.ContainsKey(G.GridDir.North));
        transform.Find("DoorIndicators/Bottom").gameObject.SetActive(doors.ContainsKey(G.GridDir.South));
        transform.Find("DoorIndicators/Left").gameObject.SetActive(doors.ContainsKey(G.GridDir.West));
        transform.Find("DoorIndicators/Right").gameObject.SetActive(doors.ContainsKey(G.GridDir.East));
    }

    public CinemachineVirtualCamera GetPlayerCamera(int playerIndex) {
        return _cameras[playerIndex];
    }

    public Vector3 GetWaypointPosition(int playerIndex) {
        return transform.position;
    }

    private void Update() {
        if (Time.time > nextMapUpdate) {
            nextMapUpdate = Time.time + Random.Range(3f, 6f);
            _itemIndicator.SetActive(HasAnyFurnitureAnItem());
        }
    }

    public bool HideItem(G.ItemType item) {
        List<Furniture> emptyRoomFurni = new List<Furniture>();
        
        foreach (Furniture furniture in furniture) {
            if (furniture.inventory == G.ItemType.None) {
                emptyRoomFurni.Add(furniture);
            }
        }

        if (emptyRoomFurni.Count == 0) {
            return false;
        }

        Furniture randomFurni = emptyRoomFurni[Random.Range(0, emptyRoomFurni.Count)]; 
        randomFurni.inventory = item;
        //Debug.Log(item.ToString() + " was placed in " + randomFurni.gameObject.name);
        return true;
    }

    public bool HasAnyFurnitureAnItem() {
        foreach (Furniture furni in furniture) {
            if (furni.inventory != G.ItemType.None) {
                return true;
            }
        }

        return false;
    }

    public bool HasAnyFurnitureEmpty() {
        foreach (Furniture furni in furniture) {
            if (furni.inventory == G.ItemType.None) {
                return true;
            }
        }

        return true;
    }
}
