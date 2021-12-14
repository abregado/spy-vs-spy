using Cinemachine;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RoomCameraSystem : MonoBehaviour {

    public Camera[] playerCameras;
    public Room[] editorRooms;

    void Start() {
        if (editorRooms.Length == 0) {
            Debug.LogError("You need to have at least one Room in the RoomCameraSystem");
        }
        
        for (int p = 0; p < 4; p++) {
            SwitchCameraToRoom(p,0);    
        }
        
    }

    public void SwitchCameraToRoom(int playerIndex, int roomIndex) {
        if (editorRooms.Length < roomIndex) {
            Debug.LogError("No room with index " + roomIndex + " found");
        }
        SetAllCameraToZero(playerIndex);
        CinemachineVirtualCamera playerCamera = editorRooms[roomIndex].GetPlayerCamera(playerIndex);
        playerCamera.Priority = 10;
    }

    public void SetAllCameraToZero(int playerIndex) {
        foreach (Room room in editorRooms) {
            CinemachineVirtualCamera playerCamera = room.GetPlayerCamera(playerIndex);
            playerCamera.Priority = 0;
        }
    }

    public Camera GetPlayerCamera(int playerIndex) {
        if (playerCameras.Length - 1 < playerIndex) {
            Debug.LogError("Not enough player cameras linked in RoomCameraSystem");
        }
        return playerCameras[playerIndex];
    }
}
