using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour {
    public int roomCount;
    public int gridWith;
    public int gridHeight;
    public int cellSize;
    public Vector3 offset;

    public Transform roomParent;
    public GameObject roomPrefab;
    public GameObject doorPrefab;
    public GameObject doorSouthPrefab;
    public GameObject windoofPrefab;
    
    private Grid<RoomTile> grid;
    private List<GameObject> rooms;

    private void Start() {
        if (roomCount > gridWith * gridHeight) {
            Debug.LogError("you cant fit more then " + gridWith * gridHeight + " rooms in your grid");
        }
        GenerateGrid();
        SpawnRooms();
        PlaceDoors();
    }

    private void ClearLevel() {
        foreach (GameObject room in rooms) {
            Destroy(room);
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
        }
    }

    private void SpawnRooms() {
        int roomsSpawned = 0;
        rooms = new List<GameObject>();
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
        rooms.Add(roomObj);
        roomsSpawned++;
        currentTile = randomNeighbour;
        if (roomsSpawned >= roomCount) {
            return;
        }
        goto NewNeighbour;
    }

    private void PlaceDoors() {
        foreach (GameObject room in rooms) {
            Room roomClass = room.GetComponent<Room>();
            Dictionary<G.GridDir, RoomTile> adjecent =
                grid.GetAdjacentOrthogonal(roomClass.myTile.gridPosition.x, roomClass.myTile.gridPosition.y);
            foreach (var kvp in adjecent) {
                if (kvp.Value == default || kvp.Value.room == null) {
                    continue;
                }
                Transform spawn = GetRandomSpawnPointForDir(kvp.Key, roomClass);
                Door door = SpawnDoor(spawn, kvp.Key);
                roomClass.doors.Add(kvp.Key, door);
            }
        }

        foreach (GameObject room in rooms) {
            Room roomClass = room.GetComponent<Room>();
            
            foreach (KeyValuePair<G.GridDir,Door> kvp in roomClass.doors) {
                Room adjecent = (grid.GetAdjecentInDir(roomClass.myTile.gridPosition.x,
                    roomClass.myTile.gridPosition.y, kvp.Key) as RoomTile).room;
                
                switch (kvp.Key) {
                    case G.GridDir.North:
                        if (!roomClass.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.South)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        roomClass.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.South];
                        break;
                    case G.GridDir.South:
                        if (!roomClass.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.North)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        roomClass.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.North];
                        break;
                    case G.GridDir.West:
                        if (!roomClass.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.East)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        roomClass.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.East];
                        break;
                    case G.GridDir.East:
                        if (!roomClass.doors.ContainsKey(kvp.Key) || !adjecent.doors.ContainsKey(G.GridDir.West)) {
                            Debug.Log("Room " + room.transform + "cant connect Door in Dir " + kvp.Key);
                        }
                        roomClass.doors[kvp.Key].doorTarget = adjecent.doors[G.GridDir.West];
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
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
