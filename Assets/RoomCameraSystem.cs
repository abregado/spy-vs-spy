using System.Linq;
using Cinemachine;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RoomCameraSystem : MonoBehaviour {

    public Transform roomParent;
    public Camera[] playerCameras;
    private Room[] rooms;
    public Room winRoom, deathRoom, welcomeRoom;

    public void Init() {

        if (roomParent == null) {
            Debug.LogError("You need a roomParent");
        }
        
        if (roomParent.childCount == 0) {
            Debug.LogError("You need to have at least one Room in the RoomParent");
        }

        rooms = new Room[roomParent.childCount];

        for (int i = 0; i < roomParent.childCount; i++) {
            rooms[i] = roomParent.GetChild(i).GetComponent<Room>();
        }
        
        for (int p = 0; p < 4; p++) {
            SwitchCameraToRoom(p,welcomeRoom);    
        }
    }

    public void SwitchCameraToRoom(int playerIndex, int roomIndex) {
        if (rooms.Length < roomIndex) {
            Debug.LogError("No room with index " + roomIndex + " found");
        }
        SetAllCameraToZero(playerIndex);
        CinemachineVirtualCamera playerCamera = rooms[roomIndex].GetPlayerCamera(playerIndex);
        playerCamera.Priority = 10;
    }
    
    public void SwitchCameraToRoom(int playerIndex, Room room) {
        foreach (Room editorRoom in rooms) {
            CinemachineVirtualCamera playerCamera = editorRoom.GetPlayerCamera(playerIndex);
            if (editorRoom != room) {
                playerCamera.Priority = 0;        
            }
        }
        welcomeRoom.GetPlayerCamera(playerIndex).Priority = 0;
        deathRoom.GetPlayerCamera(playerIndex).Priority = 0;
        winRoom.GetPlayerCamera(playerIndex).Priority = 0;
        room.GetPlayerCamera(playerIndex).Priority = 10;
    }

    public Room GetWinRoom() {
        return winRoom;
    }
    public Room GetDeathRoom() {
        return deathRoom;
    }
    public Room GetWelcomeRoom() {
        return welcomeRoom;
    }

    public void SetAllCameraToZero(int playerIndex) {
        foreach (Room room in rooms) {
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
