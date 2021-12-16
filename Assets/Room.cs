using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour {
    public bool canSpawnHere = true;

    private CinemachineVirtualCamera[] _cameras;
    private Waypoint[] _waypoints;
    public RoomTile myTile;

    // Start is called before the first frame update
    void Awake() {
        _cameras = transform.GetComponentsInChildren<CinemachineVirtualCamera>();
        _waypoints = transform.GetComponentsInChildren<Waypoint>();
    }

    public CinemachineVirtualCamera GetPlayerCamera(int playerIndex) {
        return _cameras[playerIndex];
    }

    public Vector3 GetWaypointPosition(int playerIndex) {
        return _waypoints[playerIndex].transform.position;
    }

    public bool HideItem(G.ItemType item) {
        Furniture[] roomFurniture = GetComponentsInChildren<Furniture>();

        List<Furniture> emptyRoomFurni = new List<Furniture>();
        
        foreach (Furniture furniture in roomFurniture) {
            if (furniture.inventory == G.ItemType.None) {
                emptyRoomFurni.Add(furniture);
            }
        }

        if (emptyRoomFurni.Count == 0) {
            return false;
        }

        emptyRoomFurni[Random.Range(0, emptyRoomFurni.Count)].inventory = item;
        return true;
    }

    public bool HasAnyFurnitureAnItem() {
        Furniture[] roomFurniture = GetComponentsInChildren<Furniture>();

        foreach (Furniture furni in roomFurniture) {
            if (furni.inventory != G.ItemType.None) {
                return true;
            }
        }

        return true;
    }

    public bool HasAnyFurnitureEmpty() {
        Furniture[] roomFurniture = GetComponentsInChildren<Furniture>();

        foreach (Furniture furni in roomFurniture) {
            if (furni.inventory == G.ItemType.None) {
                return true;
            }
        }

        return true;
    }
}
