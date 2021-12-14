using UnityEngine;

public class Spy: MonoBehaviour {
    public int playerIndex;
    public MeshRegistry.ItemType inventory;
    private Room _currentRoom;

    private MeshFilter _inventoryMesh;
    private MeshRenderer _inventoryRenderer;
    private MeshRegistry _meshRegistry;
    void Awake() {
        _inventoryMesh = transform.Find("Inventory").GetComponent<MeshFilter>();
        _inventoryRenderer = transform.Find("Inventory").GetComponent<MeshRenderer>();
        _meshRegistry = FindObjectOfType<MeshRegistry>();
    }
    
    void Start() {
        SetInventoryMesh();   
    }
    
    public void SetInventoryMesh() {
        _inventoryRenderer.enabled = (inventory > 0);
        _inventoryMesh.mesh = _meshRegistry.GetMesh(inventory);
    }

    public void ChangeRoom(Room targetRoom) {
        _currentRoom = targetRoom;
        transform.position = _currentRoom.GetWaypointPosition(playerIndex);
    }
}
