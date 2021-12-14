using UnityEngine;

public class Spy: MonoBehaviour {
    public int playerIndex;
    public int inventory;
    private Room _currentRoom;

    private MeshFilter _inventoryMesh;
    private MeshRenderer _inventoryRenderer;
    void Awake() {
        _inventoryMesh = transform.Find("Inventory").GetComponent<MeshFilter>();
        _inventoryRenderer = transform.Find("Inventory").GetComponent<MeshRenderer>();
    }
    
    void Start() {
        SetInventoryMesh();   
    }
    
    public void SetInventoryMesh() {
        _inventoryRenderer.enabled = (inventory > 0);
    }

    public void ChangeRoom(Room targetRoom) {
        _currentRoom = targetRoom;
        transform.position = _currentRoom.GetWaypointPosition(playerIndex);
    }
}
