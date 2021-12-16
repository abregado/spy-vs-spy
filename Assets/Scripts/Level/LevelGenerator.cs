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

    public GameObject roomPrefab;
    
    private Grid<RoomTile> grid;
    private List<GameObject> rooms;

    private void Start() {
        if (roomCount > gridWith * gridHeight) {
            Debug.LogError("you cant fit more then " + gridWith * gridHeight + " rooms in your grid");
        }
        
        
        
        GenerateGrid();
        SpawnRooms();
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
            Quaternion.identity);
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

    private void GenerateGrid() {
        grid = new Grid<RoomTile>(gridWith, gridHeight, cellSize, offset, 
            (Grid<RoomTile> g, int x, int z) => new RoomTile(g, new Vector2Int(x, z)));
        
        
    }
}
