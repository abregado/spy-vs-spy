using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour {
    public int depthLayer;
    public Vector2Int mapPosition;
    
    private CinemachineVirtualCamera[] _cameras;
    private Waypoint[] _waypoints;

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
}
