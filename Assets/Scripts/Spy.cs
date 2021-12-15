using Rewired;
using UnityEngine;

public class Spy: MonoBehaviour {
    public int playerIndex;
    public MeshRegistry.ItemType inventory;
    private Room _currentRoom;

    private MeshFilter _inventoryMesh;
    private MeshRenderer _inventoryRenderer;
    private MeshRegistry _meshRegistry;
    
    //Movement Stuff
    public float moveSpeed = 3.0f;
    private Player player; // The Rewired Player
    private CharacterController cc;
    private Vector3 _moveVector;
    private bool _interact;
    private bool _set;
    private bool _attack;
    private bool _map;
    
    void Awake() {
        _inventoryMesh = transform.Find("Inventory").GetComponent<MeshFilter>();
        _inventoryRenderer = transform.Find("Inventory").GetComponent<MeshRenderer>();
        _meshRegistry = FindObjectOfType<MeshRegistry>();
        
        player = ReInput.players.GetPlayer(playerIndex);
        cc = GetComponent<CharacterController>();
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
    
    void Update () {
        GetInput();
        ProcessInput();
    }

    private void GetInput() {
        _moveVector.x = player.GetAxis("MoveHorizontal");
        _moveVector.y = player.GetAxis("MoveVertical");
        _interact = player.GetButtonDown("Interact");
        _set = player.GetButtonDown("Set");
        _attack = player.GetButtonDown("Attack");
        _map = player.GetButtonDown("Map");
    }

    private void ProcessInput() {
        if(_moveVector.x != 0.0f || _moveVector.y != 0.0f) {
            cc.Move(_moveVector * moveSpeed * Time.deltaTime);
        }
        // Debug.Log("-------------");
        if (_interact) Debug.Log("Interact");
        if (_set) Debug.Log("Set");
        if (_attack) Debug.Log("Attack");
        if (_map) Debug.Log("Map");
        // Debug.Log("-------------");
    }
}
