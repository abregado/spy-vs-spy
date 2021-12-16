using System;
using System.Collections;
using Rewired;
using UnityEngine;
using UnityEngine.Serialization;

public class Spy: MonoBehaviour {
    public int playerIndex;
    public G.ItemType inventory;
    public Room currentRoom;

    public GameObject _stabPrefab;

    private MeshFilter _inventoryMesh;
    private MeshRenderer _inventoryRenderer;
    private MeshRegistry _meshRegistry;
    private Collider _collider;
    private ParticleSystem _trapDeathParticles;
    private ParticleSystem _spyDeathParticles;

    public int healthPoints;
    private float currentMoveSpeed = 3f;
    private float speedResetTimer;
    
    //Movement Stuff
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
    private int _spyLayerMask = 1 << 10;

    public bool isAlive;
    public bool isPlaying;
    private bool _hasMadeInput;
    private float _respawnTimer;

    public void Init() {
        _itemHider = FindObjectOfType<ItemHider>();
        _cameraSystem = FindObjectOfType<RoomCameraSystem>();
        _inventoryMesh = transform.Find("Inventory").GetComponent<MeshFilter>();
        _inventoryRenderer = transform.Find("Inventory").GetComponent<MeshRenderer>();
        _meshRegistry = FindObjectOfType<MeshRegistry>();
        _handler = FindObjectOfType<SpyHandler>();
        _collider = GetComponent<Collider>();
        _trapDeathParticles = transform.Find("DomeNukeRed")?.GetComponent<ParticleSystem>();
        _spyDeathParticles = transform.Find("BloodBoneExplosionBig")?.GetComponent<ParticleSystem>();

        player = ReInput.players.GetPlayer(playerIndex);
        cc = GetComponent<CharacterController>();
    }
    
    public void StartSpy() {
        SetInventoryMesh();
        isPlaying = false;
        isAlive = false;
        _hasMadeInput = false;
        SetVisible(false);
        if (_trapDeathParticles != null) {
            _trapDeathParticles.Stop();
        }
        if (_spyDeathParticles != null) {
            _spyDeathParticles.Stop();
        }
    }

    private void Explode() {
        if (_trapDeathParticles != null) {
            _trapDeathParticles.Play();
        }
    }

    private void BloodExplode() {
        if (_spyDeathParticles != null) {
            _spyDeathParticles.Play();
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
        if (currentMoveSpeed < G.MAX_MOVE_SPEED && Time.time > speedResetTimer) {
            currentMoveSpeed = G.MAX_MOVE_SPEED;
        }
        
        if (isAlive == false && isPlaying == false && _hasMadeInput) {
            Respawn();
            isPlaying = true;
            return;
        }
        
        if (isPlaying && isAlive == false && Time.time > _respawnTimer) {
            Respawn();
            return;
        }
        
        GetInput();
        ProcessInput();    
        
        if (isAlive && isPlaying) {
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
            Vector3 frameVector = _moveVector.normalized * currentMoveSpeed * Time.deltaTime * -1f;
            Vector3 actualMoveVector = new Vector3(frameVector.x, 0f, frameVector.y);
            cc.Move(actualMoveVector);
            transform.LookAt(transform.position+actualMoveVector);
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
    }

    private void ProcessInput() {
        // Debug.Log("-------------");
        if (_interact) {
            _hasMadeInput = true;
            if (isAlive) {
                Interact();
            }

            //Debug.Log("Interact");
        }

        if (_set) {
            _hasMadeInput = true;
            if (isAlive) {
                SetTrap();
            }
            //Debug.Log("Set");
        }

        if (_attack) {
            _hasMadeInput = true;
            if (isAlive) {
                AttackNearest();
            }
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

    private void AttackNearest() {
        Spy nearestSpy = GetClosestOtherSpy();
        if (nearestSpy != null) {
            DamageTarget(nearestSpy);
        }
        else {
            //SlashAnimation(transform.position+(transform.forward*0.5f)+Vector3.up);
        }

        currentMoveSpeed = G.ATTACK_MOVE_SPEED;
        speedResetTimer = Time.time + G.SPEED_RESET_TIME;
    }

    public void DamageTarget(Spy spy) {
        Vector3 middlePoint = (spy.transform.position - transform.position)/2f + transform.position + Vector3.up ;
        SlashAnimation(middlePoint);
        int damage = 2;
        if (inventory == G.ItemType.None) {
            damage = 3;
        }
        spy.Hurt(damage,this);
    }

    private void SlashAnimation(Vector3 position) {
        if (_stabPrefab != null) {
            Instantiate(_stabPrefab, position, Quaternion.identity, transform);
        }
    }

    public void Hurt(int damage,Spy attacker) {
        healthPoints -= damage;
        if (healthPoints <= 1) {
            KillBySpy(attacker);
        }

        currentMoveSpeed = G.DAMAGED_MOVE_SPEED;
        speedResetTimer = Time.time + (G.SPEED_RESET_TIME/2f);
    }
    
    public Spy GetClosestOtherSpy() {
        Vector3 frontPos = transform.position + (transform.forward * 0.25f);
        Collider[] inRange = Physics.OverlapSphere(frontPos, 0.75f,_spyLayerMask);
        
        Spy closest = null;

        float closestDistance = 1000f;

        foreach (Collider collider in inRange) {
            GameObject colliderObject = collider.gameObject;
            Spy spy = colliderObject.GetComponent<Spy>();
            float distanceToTarget = Vector3.Distance(colliderObject.transform.position, frontPos);
            if (spy != null && spy != this && spy.isAlive && distanceToTarget < closestDistance)
                if (colliderObject != gameObject) {
                    closest = spy;
                    closestDistance = distanceToTarget;
                }
        }

        return closest;
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
        isAlive = false;
        isPlaying = false;
        SetVisible(false);
        StartCoroutine(nameof(ChangeCameraToDeathRoom));
        Explode();
        _collider.enabled = false;
        SetInventoryMesh();
    }
    
    public void KillByTrap(Room room) {
        _respawnTimer = Time.time + G.RESPAWN_TIME;
        isAlive = false;
        SetVisible(false);
        StartCoroutine(nameof(ChangeCameraToDeathRoom));
        Explode();
        _collider.enabled = false;
        
        if (room.HasAnyFurnitureEmpty()) {
            room.HideItem(inventory);
            inventory = G.ItemType.None;
        }
        else {
            _itemHider.HideItemAnywhere(inventory);
            inventory = G.ItemType.None;
        }
        SetInventoryMesh();
    }

    public void KillBySpy(Spy killer) {
        _respawnTimer = Time.time + G.RESPAWN_TIME;
        isAlive = false;
        SetVisible(false);
        StartCoroutine(nameof(ChangeCameraToDeathRoom));
        BloodExplode();

        if (inventory != G.ItemType.None && killer.inventory == G.ItemType.None) {
            killer.inventory = inventory;
            inventory = G.ItemType.None;
            killer.SetInventoryMesh();
        }
        else {
            if (currentRoom.HasAnyFurnitureEmpty()) {
                currentRoom.HideItem(inventory);
                inventory = G.ItemType.None;
            }
            else {
                _itemHider.HideItemAnywhere(inventory);
                inventory = G.ItemType.None;
            }
        }
        SetInventoryMesh();
    }

    private IEnumerator ChangeCameraToDeathRoom() {
        yield return new WaitForSeconds(G.TIME_BEFORE_DEATH_ROOM_CHANGE);
        _cameraSystem.SwitchCameraToRoom(playerIndex,_cameraSystem.GetDeathRoom());
    }

    public void GotoWinRoom() {
        Room exitRoom = _cameraSystem.GetWinRoom();

        cc.enabled = false;
        transform.position = exitRoom.GetWaypointPosition(playerIndex);
        cc.enabled = true;

        currentRoom = exitRoom;
        _cameraSystem.SwitchCameraToRoom(playerIndex,2);
        SetVisible(true);
        isAlive = true;
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
        isAlive = true;
        healthPoints = G.MAX_HEALTH;
        SetInventoryMesh();
    }

    private void SetVisible(bool state) {
        transform.Find("View").gameObject.SetActive(state);
        _collider.enabled = state;
    }
}
