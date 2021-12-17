using System.Linq;
using Cinemachine;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RoomCameraSystem : MonoBehaviour {

    public Transform roomParent;
    public Camera[] playerCameras;
    private Room[] rooms;
    public Room winRoom, deathRoom, welcomeRoom, mapRoom;

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
        
        for (int i = 0; i < 4; i++) {
            mapRoom.GetPlayerCamera(i).m_Lens.OrthographicSize = 25f;
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
        mapRoom.GetPlayerCamera(playerIndex).Priority = 0;
        room.GetPlayerCamera(playerIndex).Priority = 10;
    }

    public void SetCameraProjectionOrtho(int playerIndex,bool state) {
        playerCameras[playerIndex].orthographic = state;
    }

    public void SetCameraToMapMode(int playerIndex, bool mapModeOn) {
        LayerMask mask = LayerMask.GetMask();
        if (mapModeOn) {
            mask = LayerMask.GetMask("MiniMap");
        } else {
            if (playerIndex == 0) {
                mask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", 
                    "Clickable", "Water", "UI", "Player1Camera");
            }
            if (playerIndex == 1) {
                mask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", 
                    "Clickable", "Water", "UI", "Player2Camera");
            }
            if (playerIndex == 2) {
                mask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", 
                    "Clickable", "Water", "UI", "Player3Camera");
            }
            if (playerIndex == 3) {
                mask = LayerMask.GetMask("Default", "TransparentFX", "Ignore Raycast", 
                    "Clickable", "Water", "UI", "Player4Camera");
            }
            
        }

        playerCameras[playerIndex].cullingMask = mask.value;
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

    public Room GetMapRoom() {
        return mapRoom;
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
