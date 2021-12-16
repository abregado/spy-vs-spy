using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {
    private GameManager _gameManager;
    
    public int roomCount;
    public int gridWith;
    public int gridHeight;
    public int cellSize;
    public Vector3 offset;

    public int exitCount;
    public int extraFurnitureCount;

    public Transform roomParent;
    public GameObject roomPrefab;
    public GameObject doorPrefab;
    public GameObject doorSouthPrefab;
    public GameObject windoofPrefab;
    
    private Grid<RoomTile> grid;
    private List<Room> rooms = new List<Room>();
    private List<Furniture> placeableFurniture = new List<Furniture>();

    public List<ExitDoor> exitDoors = new List<ExitDoor>();

    public void Init(GameManager gameManager) {
        _gameManager = gameManager;
    }
    
    public void BuildMap() {
        ClearLevel();
        if (roomCount > gridWith * gridHeight) {
            Debug.LogError("you cant fit more then " + gridWith * gridHeight + " rooms in your grid");
        }
        GenerateGrid();
        SpawnRooms();
        PlaceDoors();
        PlaceExits();
        PlaceFurniture();
    }

    private void ClearLevel() {
        foreach (Room room in rooms) {
            Destroy(room.gameObject);
        }
        rooms.Clear();
        grid = null;
    }
    

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ClearLevel();
            GenerateGrid();
            SpawnRooms();
            PlaceDoors();
            PlaceExits();
            PlaceFurniture();
        }
    }

    private void PlaceFurniture() {
        foreach (Room room in rooms) {
            if (room.doors.Count < 2) {
                Transform spawn = GetRandomSpawnPointForFurniture(room);
                GameObject prefab = GetRandomPlaceableFurniture();
                GameObject obj = Instantiate(prefab, spawn);
                placeableFurniture.Add(obj.GetComponent<Furniture>());
            }
        }

        for (int i = 0; i < extraFurnitureCount; i++) {
            
        }
    }

    private GameObject GetRandomPlaceableFurniture() {
        PickCandidate:
        GameObject candidate = _gameManager.meshReg.GetRandomFurniture();
        if (candidate.GetComponent<Furniture>() == null) {
            goto PickCandidate;
        }

        return candidate;
    }

    private void SpawnRooms() {
        int roomsSpawned = 0;
        rooms = new List<Room>();
        RoomTile currentTile = grid._gridArray[Random.Range(0, gridWith), Random.Range(0, gridHeight)];

        RandomRoom:
        if (roomsSpawned > 0) {
            currentTile = rooms[Random.Range(0, rooms.Count)].GetComponent<Room>().myTile;
            Debug.Log("RandomRoom");
            if (!grid.HasNeighbourCount(currentTile.gridPosition.x, currentTile.gridPosition.y, 1)) {
                goto RandomRoom;
            }
        }

        NewNeighbour:
        List<RoomTile> neighbours = grid.GetAdjacentOrthogonalFreeRoomTiles(currentTile.gridPosition.x, currentTile.gridPosition.y);
        if (neighbours.Count <= 0) {
            goto RandomRoom;
        }
        RoomTile randomNeighbour = neighbours[Random.Range(0, neighbours.Count)];
        GameObject roomObj = Instantiate(roomPrefab,
            grid.GetWorldPositionCenter(randomNeighbour.gridPosition.x, randomNeighbour.gridPosition.y),
            Quaternion.identity, roomParent);
        randomNeighbour.room = roomObj.GetComponent<Room>();
        randomNeighbour.room.myTile = randomNeighbour;
        rooms.Add(roomObj.GetComponent<Room>());
        roomsSpawned++;
        currentTile = randomNeighbour;
        if (roomsSpawned >= roomCount) {
            return;
        }
        goto NewNeighbour;
    }

    private void PlaceExits() {
        List<(G.GridDir, Room)> possibleExits = new List<(G.GridDir, Room)>();
        foreach (Room room in rooms) {
            if (!room.doors.ContainsKey(G.GridDir.South)) {
                possibleExits.Add((G.GridDir.South, room));
            }
            if (!room.doors.ContainsKey(G.GridDir.East)) {
                possibleExits.Add((G.GridDir.East, room));
            }
            if (!room.doors.ContainsKey(G.GridDir.West)) {
                possibleExits.Add((G.GridDir.West, room));
            }
        }
        
        possibleExits.Shuffle();

        for (int i = 0; i < exitCount; i++) {
            Transform spawn = GetSpawnPointForExit(possibleExits[i].Item1, possibleExits[i].Item2);
            ExitDoor exit = SpawnExit(spawn, possibleExits[i].Item1);
            exitDoors.Add(exit);
        }
    }

    private void PlaceDoors() {
        foreach (Room room in rooms) {
            Dictionary<G.GridDir, RoomTile> adjecent =
                grid.GetAdjacentOrthogonal(room.myTile.gridPosition.x, room.myTile.gridPosition.y);
            foreach (var kvp in adjecent) {
                if (kvp.Value == default || kvp.Value.room == null) {
                    continue;
                }
                Transform spawn = GetRandomSpawnPointForDir(kvp.Key, room);
                Door door = SpawnDoor(spawn, kvp.Key);
                room.doors.Add(kvp.Key, door);
            }
        }

        foreach (Room room in rooms) {
            foreach (KeyValuePair<G.GridDir,Door> kvp in room.doors) {
                Room adjecent = (grid.GetAdjecentInDir(room.myTile.gridPosition.x,
                    room.myTile.gridPosition.y, kvp.Key) as RoomTile).room;
                
                switch (kvp.Key) {
                    case G.GridDir.North:
                        if (!room.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.South)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        room.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.South];
                        break;
                    case G.GridDir.South:
                        if (!room.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.North)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        room.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.North];
                        break;
                    case G.GridDir.West:
                        if (!room.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.East)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        room.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.East];
                        break;
                    case G.GridDir.East:
                        if (!room.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.West)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        room.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.West];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private ExitDoor SpawnExit(Transform parent, G.GridDir dir) {
        GameObject door = null;
        switch (dir) {
            // case G.GridDir.North:
            //     door = Instantiate(doorSouthPrefab, parent);
            //     break;
            case G.GridDir.South:
                door = Instantiate(windoofPrefab, parent);
                break;
            case G.GridDir.West:
                door = Instantiate(windoofPrefab, parent);
                break;
            case G.GridDir.East:
                door = Instantiate(windoofPrefab, parent);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        return door.GetComponent<ExitDoor>();
    }
    
    private Door SpawnDoor(Transform parent, G.GridDir dir) {
        GameObject door = null;
        switch (dir) {
            case G.GridDir.North:
                door = Instantiate(doorSouthPrefab, parent);
                break;
            case G.GridDir.South:
                door = Instantiate(doorPrefab, parent);
                break;
            case G.GridDir.West:
                door = Instantiate(doorPrefab, parent);
                break;
            case G.GridDir.East:
                door = Instantiate(doorPrefab, parent);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        return door.GetComponent<Door>();
    }

    private Transform GetRandomSpawnPointForFurniture(Room room) {
        Transform spawnParent = room.transform.Find("Spawnpoints").transform;
        List<Transform> freeSpawns = new List<Transform>();

        for (int i = 0; i < spawnParent.childCount; i++) {
            Transform current = spawnParent.GetChild(i);
            if (current.name == "Center") {
                continue;
            }

            if (current.childCount > 0) {
                continue;
            }
            
            freeSpawns.Add(current);
            freeSpawns.Shuffle();
        }

        if (freeSpawns.Count == 0) {
            return null;
        }
        return freeSpawns[0];
    }

    private Transform GetSpawnPointForExit(G.GridDir dir, Room room) {
        Transform spawns = room.transform.Find("Spawnpoints").transform;
        
        switch (dir) {
            case G.GridDir.South:
                return spawns.Find("Bottom_middle");
            case G.GridDir.West:
                return spawns.Find("Left_middle");
            case G.GridDir.East:
                return spawns.Find("Right_middle");
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    private Transform GetRandomSpawnPointForDir(G.GridDir dir, Room room) {
        Transform spawns = room.transform.Find("Spawnpoints").transform;
        
        int index = Random.Range(0, 3);
        switch (dir) {
            case G.GridDir.North:
                switch (index) {
                    case 0:
                        return spawns.Find("Top_left");
                    case 1:
                        return spawns.Find("Top_middle");
                    case 2:
                        return spawns.Find("Top_right");
                    default:
                        Debug.Log("Wrong Random Index");
                        break;
                }
                break;
            case G.GridDir.South:
                switch (index) {
                    case 0:
                        return spawns.Find("Bottom_left");
                    case 1:
                        return spawns.Find("Bottom_middle");
                    case 2:
                        return spawns.Find("Bottom_right");
                    default:
                        Debug.Log("Wrong Random Index");
                        break;
                }
                break;
            case G.GridDir.West:
                switch (index) {
                    case 0:
                        return spawns.Find("Left_top");
                    case 1:
                        return spawns.Find("Left_middle");
                    case 2:
                        return spawns.Find("Left_bottom");
                    default:
                        Debug.Log("Wrong Random Index");
                        break;
                }
                break;
            case G.GridDir.East:
                switch (index) {
                    case 0:
                        return spawns.Find("Right_top");
                    case 1:
                        return spawns.Find("Right_middle");
                    case 2:
                        return spawns.Find("Right_bottom");
                    default:
                        Debug.Log("Wrong Random Index");
                        break;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }

        return null;
    }

    private void GenerateGrid() {
        grid = new Grid<RoomTile>(gridWith, gridHeight, cellSize, offset, 
            (Grid<RoomTile> g, int x, int z) => new RoomTile(g, new Vector2Int(x, z)));
        
        
    }
    
   
}
