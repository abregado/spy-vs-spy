using System;
using System.Collections;
using Rewired;
using UnityEngine;
using UnityEngine.Serialization;

public class Spy: MonoBehaviour {
    public int playerIndex;
    public MeshRegistry.ItemType inventory;
    public Room currentRoom;

    private MeshFilter _inventoryMesh;
    private MeshRenderer _inventoryRenderer;
    private MeshRegistry _meshRegistry;
    private Collider _collider;
    private ParticleSystem _trapDeathParticles;
    
    //Movement Stuff
    public float moveSpeed = 3.0f;
    private Player player; // The Rewired Player
    private CharacterController cc;
    private Vector3 _moveVector;
    private bool _interact;
    private bool _set;
    private bool _attack;
    private bool _map;
    private bool _isTeleporting;
    private Door _teleportDestination;

    private RoomCameraSystem _cameraSystem;
    private ItemHider _itemHider;
    private SpyHandler _handler;
    private int _interactionLayerMask = 1 << 3;

    private bool _isAlive;
    public bool isPlaying;
    private bool _hasMadeInput;
    private float _respawnTimer;
    private const float RESPAWN_TIME = 30f;
    
    void Awake() {
        _itemHider = FindObjectOfType<ItemHider>();
        _cameraSystem = FindObjectOfType<RoomCameraSystem>();
        _inventoryMesh = transform.Find("Inventory").GetComponent<MeshFilter>();
        _inventoryRenderer = transform.Find("Inventory").GetComponent<MeshRenderer>();
        _meshRegistry = FindObjectOfType<MeshRegistry>();
        _handler = FindObjectOfType<SpyHandler>();
        _collider = GetComponent<Collider>();
        _trapDeathParticles = transform.Find("DomeNukeRed").GetComponent<ParticleSystem>();

        player = ReInput.players.GetPlayer(playerIndex);
        cc = GetComponent<CharacterController>();
    }
    
    void Start() {
        SetInventoryMesh();
        isPlaying = false;
        _isAlive = false;
        _hasMadeInput = false;
        SetVisible(false);
        if (_trapDeathParticles != null) {
            _trapDeathParticles.Stop();
        }
    }

    private void Explode() {
        if (_trapDeathParticles != null) {
            _trapDeathParticles.Play();
        }
    }
    
    public void SetInventoryMesh() {
        _inventoryRenderer.enabled = (inventory > 0);
        _inventoryMesh.mesh = _meshRegistry.GetMesh(inventory);
    }

    public void GoToDoor(Door door) {
        _teleportDestination = door;
        _isTeleporting = true;
        
    }
    
    void Update () {
        if (_isAlive == false && isPlaying == false && _hasMadeInput) {
            Respawn();
            isPlaying = true;
            return;
        }
        
        if (isPlaying && _isAlive == false && Time.time > _respawnTimer) {
            Respawn();
            return;
        }
        
        GetInput();
        ProcessInput();    
        
        if (_isAlive && isPlaying) {
            if (_isTeleporting == false) {
                ProcessMovement();
            }
            else {
                CompleteTeleport();
            }
        }

        
    }


    private void CompleteTeleport() {
        currentRoom = _teleportDestination.myRoom;
        cc.enabled = false;
        transform.SetPositionAndRotation(_teleportDestination.GetExitPosition(),Quaternion.identity);
        cc.enabled = true;
        _isTeleporting = false;
        _cameraSystem.SwitchCameraToRoom(playerIndex,currentRoom);
    }

    private void GetInput() {
        _moveVector.x = player.GetAxis("MoveHorizontal");
        _moveVector.y = player.GetAxis("MoveVertical");
        _interact = player.GetButtonDown("Interact");
        _set = player.GetButtonDown("Set");
        _attack = player.GetButtonDown("Attack");
        _map = player.GetButtonDown("Map");
    }

    private void ProcessMovement() {
        if(_moveVector.x != 0.0f || _moveVector.y != 0.0f) {
            Vector3 frameVector = _moveVector.normalized * moveSpeed * Time.deltaTime * -1f;
            Vector3 actualMoveVector = new Vector3(frameVector.x, 0f, frameVector.y);
            cc.Move(actualMoveVector);
            transform.LookAt(transform.position+actualMoveVector);
        }
    }

    private void ProcessInput() {
        // Debug.Log("-------------");
        if (_interact) {
            _hasMadeInput = true;
            if (_isAlive) {
                Interact();
            }

            //Debug.Log("Interact");
        }

        if (_set) {
            _hasMadeInput = true;
            if (_isAlive) {
                SetTrap();
            }
            //Debug.Log("Set");
        }

        if (_attack) {
            _hasMadeInput = true;
            //Debug.Log("Attack");
        }

        if (_map) {
            _hasMadeInput = true;
            //Debug.Log("Map");
        }
        // Debug.Log("-------------");
    }

    private void Interact() {
        IInteractable interactable = GetClosestInteractable();
        if (interactable != null) {
            interactable.OnInteract(this);
        }
    }

    private void SetTrap() {
        ICanBeTrapped trappable = GetClosestTrappable();
        if (trappable != null) {
            trappable.OnTrapSet(this);
        }
    }
    
    public IInteractable GetClosestInteractable() {
        Vector3 frontPos = transform.position + (transform.forward * 0.25f);
        Collider[] inRange = Physics.OverlapSphere(frontPos, 0.25f,_interactionLayerMask);
        
        IInteractable closest = null;

        float closestDistance = 1000f;

        foreach (Collider collider in inRange) {
            GameObject colliderObject = collider.gameObject;
            IInteractable interactable = colliderObject.GetComponent<IInteractable>();
            float distanceToTarget = Vector3.Distance(colliderObject.transform.position, frontPos);
            if (interactable != null && interactable.interactable && distanceToTarget < closestDistance)
                if (colliderObject != gameObject) {
                    closest = interactable;
                    closestDistance = distanceToTarget;
                }
        }

        return closest;
    }
    
    public ICanBeTrapped GetClosestTrappable() {
        Vector3 frontPos = transform.position + (transform.forward * 0.25f);
        Collider[] inRange = Physics.OverlapSphere(frontPos, 0.25f,_interactionLayerMask);
        
        ICanBeTrapped closest = null;

        float closestDistance = 1000f;

        foreach (Collider collider in inRange) {
            GameObject colliderObject = collider.gameObject;
            ICanBeTrapped trappable = colliderObject.GetComponent<ICanBeTrapped>();
            float distanceToTarget = Vector3.Distance(colliderObject.transform.position, frontPos);
            if (trappable != null && trappable.trappable && distanceToTarget < closestDistance)
                if (colliderObject != gameObject) {
                    closest = trappable;
                    closestDistance = distanceToTarget;
                }
        }

        return closest;
    }

    public void KillByWin() {
        _isAlive = false;
        isPlaying = false;
        SetVisible(false);
        StartCoroutine(nameof(ChangeCameraToDeathRoom));
        Explode();
        _collider.enabled = false;
    }
    
    public void KillByTrap(Room room) {
        _respawnTimer = Time.time + RESPAWN_TIME;
        _isAlive = false;
        SetVisible(false);
        StartCoroutine(nameof(ChangeCameraToDeathRoom));
        Explode();
        _collider.enabled = false;
        
        if (room.HasAnyFurnitureEmpty()) {
            room.HideItem(inventory);
            inventory = MeshRegistry.ItemType.None;
        }
        else {
            _itemHider.HideItemAnywhere(inventory);
            inventory = MeshRegistry.ItemType.None;
        }
        
    }

    public void KillBySpy(Spy spy) {
        _respawnTimer = Time.time + RESPAWN_TIME;
        _isAlive = false;
        SetVisible(false);
        StartCoroutine(nameof(ChangeCameraToDeathRoom));
        Explode();

        if (inventory != MeshRegistry.ItemType.None && spy.inventory == MeshRegistry.ItemType.None) {
            spy.inventory = inventory;
            inventory = MeshRegistry.ItemType.None;
        }
        else {
            if (currentRoom.HasAnyFurnitureEmpty()) {
                currentRoom.HideItem(inventory);
                inventory = MeshRegistry.ItemType.None;
            }
            else {
                _itemHider.HideItemAnywhere(inventory);
                inventory = MeshRegistry.ItemType.None;
            }
        }
    }

    private IEnumerator ChangeCameraToDeathRoom() {
        yield return new WaitForSeconds(3);
        _cameraSystem.SwitchCameraToRoom(playerIndex,1);
    }

    public void GotoWinRoom() {
        Room exitRoom = _cameraSystem.GetWinRoom();

        cc.enabled = false;
        transform.position = exitRoom.GetWaypointPosition(playerIndex);
        cc.enabled = true;

        currentRoom = exitRoom;
        _cameraSystem.SwitchCameraToRoom(playerIndex,2);
        SetVisible(true);
        _isAlive = true;
        isPlaying = false;
        _handler.KillAllSpiesExcept(playerIndex);
    }

    private void Respawn() {
        Room emptyRoom = _handler.GetEmptyRoomForRespawn();

        cc.enabled = false;
        transform.position = emptyRoom.GetWaypointPosition(playerIndex);
        cc.enabled = true;

        currentRoom = emptyRoom;
        _cameraSystem.SwitchCameraToRoom(playerIndex,currentRoom);
        SetVisible(true);
        _isAlive = true;
    }

    private void SetVisible(bool state) {
        transform.Find("View").gameObject.SetActive(state);
        _collider.enabled = state;
    }
}
