using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid<TGridObject> {

    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPos;
    public TGridObject[,] _gridArray;

    private readonly Vector3 centerOffset;

    public Grid(int width, int height, float cellSize, Vector3 originPos, Func<Grid<TGridObject>, int, int, TGridObject> gridObj) {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPos = originPos;
        _gridArray = new TGridObject[width, height];
        centerOffset = new Vector3(cellSize, 0, cellSize) * 0.5f;

        for (int x = 0; x < _gridArray.GetLength(0); x++) {
            for (int z = 0; z < _gridArray.GetLength(1); z++) {
                _gridArray[x, z] = gridObj(this, x, z);
            }
        }
        // Debug.Log("Build Grid<" + typeof(TGridObject) + ">: " + width + ", " + height + " - Size " + cellSize);
    }

    public TGridObject[] GetGridObjectsInRadius(Vector3 center, float radius) {
        List<TGridObject> objs = new List<TGridObject>();
        objs.Add(GetGridObject(center + new Vector3(radius, 0, 0)));
        objs.Add(GetGridObject(center + new Vector3(-radius, 0, 0)));
        objs.Add(GetGridObject(center + new Vector3(0, 0, radius)));
        objs.Add(GetGridObject(center + new Vector3(0, 0, -radius)));
        return objs.ToArray();
    }

    public TGridObject GetGridObject(Vector3 worldPos) {
        int x, z;
        GetGridPosition(worldPos, out x, out z);
        return GetGridObject(x, z);
    }
    public TGridObject GetGridObject(int x, int z) {
        if (x >= 0 && z >= 0 && x < _width && z < _height) {
            return _gridArray[x, z];
        }

        return default(TGridObject);
    }

    public Dictionary<G.GridDir, TGridObject> GetAdjacentOrthogonal(int x, int z) {
        Dictionary<G.GridDir, TGridObject> objs = new Dictionary<G.GridDir, TGridObject>(4);
        objs[G.GridDir.East] = GetGridObject(x + 1, z);
        objs[G.GridDir.West] = GetGridObject(x - 1, z);
        objs[G.GridDir.North] = GetGridObject(x, z + 1);
        objs[G.GridDir.South] = GetGridObject(x, z - 1);
        return objs;
    }
    
    public List<RoomTile> GetAdjacentOrthogonalFreeRoomTiles(int x, int z) {
        List<RoomTile> objs = new List<RoomTile>();
        RoomTile tileEast = GetGridObject(x + 1, z) as RoomTile;
        if (tileEast != null && tileEast.room == null) {
            objs.Add(tileEast);
        }
        
        RoomTile tileWest = GetGridObject(x - 1, z) as RoomTile;
        if (tileWest != null && tileWest.room == null) {
            objs.Add(tileWest);
        }
        
        RoomTile tileNorth = GetGridObject(x, z + 1) as RoomTile;
        if (tileNorth != null && tileNorth.room == null) {
            objs.Add(tileNorth);
        }
        
        RoomTile tileSouth = GetGridObject(x, z - 1) as RoomTile;
        if (tileSouth != null && tileSouth.room == null) {
            objs.Add(tileSouth);
        }
        return objs;
    }

    public bool HasNeighbourCount(int x, int z, int countGoal) {
        Dictionary<G.GridDir, TGridObject> neighbours = GetAdjacentOrthogonal(x, z);
        int count = 0;
        foreach (KeyValuePair<G.GridDir,TGridObject> keyValuePair in neighbours) {
            if (keyValuePair.Value is RoomTile tile) {
                if (tile.room != null) {
                    count++;
                }
            }
        }

        return count >= countGoal;
    }
    
    public Dictionary<G.GridDir, TGridObject> GetAdjacentRing(int x, int z) {
        Dictionary<G.GridDir, TGridObject> objs = new Dictionary<G.GridDir, TGridObject>(8);
        objs[G.GridDir.East] = GetGridObject(x + 1, z);
        objs[G.GridDir.West] = GetGridObject(x - 1, z);
        objs[G.GridDir.North] = GetGridObject(x, z + 1);
        objs[G.GridDir.South] = GetGridObject(x, z - 1);
        objs[G.GridDir.NorthEast] = GetGridObject(x + 1, z + 1);
        objs[G.GridDir.NorthWest] = GetGridObject(x - 1, z + 1);
        objs[G.GridDir.SouthEast] = GetGridObject(x + 1, z - 1);
        objs[G.GridDir.SouthWest] = GetGridObject(x - 1, z - 1);
        return objs;
    }
    
    public void SetGridObject(Vector3 worldPos, TGridObject obj) {
        int x, z;
        GetGridPosition(worldPos, out x, out z);
        SetGridObject(x, z, obj);
    }
    
    public void SetGridObject(int x, int z, TGridObject obj) {
        if (x >= 0 && z >= 0 && x < _width && z < _height) {
            _gridArray[x, z] = obj;
        }
    }

    // public List<EmergeTile> GetTilesInLineBetween(EmergeTile startTile, EmergeTile endTile) {
    //     bool xDir = startTile.x != endTile.x;
    //     bool zDir = startTile.z != endTile.z;
    //     if (xDir && zDir) {
    //         ErrorLog("Tiles are not in Line");
    //         return null;
    //     }
    //     if (!xDir && !zDir) {
    //         ErrorLog("Tiles are the same");
    //         return null;
    //     }
    //     
    //     List<EmergeTile> tiles = new List<EmergeTile>();
    //     int from = xDir ? startTile.x : startTile.z;
    //     int to = xDir ? endTile.x : endTile.z;
    //
    //     bool forward = from <= to;
    //     if (forward) {
    //         for (int value = from + 1; value < to; value++) {
    //             if (xDir) {
    //                 tiles.Add(_gridArray[value, startTile.z] as EmergeTile);  
    //             } else {
    //                 tiles.Add(_gridArray[startTile.x, value] as EmergeTile);
    //             }
    //         }
    //     } else {
    //         for (int value = from - 1; value > to; value--) {
    //             if (xDir) {
    //                 tiles.Add(_gridArray[value, startTile.z] as EmergeTile);  
    //             } else {
    //                 tiles.Add(_gridArray[startTile.x, value] as EmergeTile);
    //             }
    //         }
    //     }
    //
    //     return tiles;
    // }
    //
    // public List<EmergeTile> GetInsideTilesArea(EmergeTile startCorner, EmergeTile endCorner) {
    //     if (startCorner.x == endCorner.x || startCorner.z == endCorner.z) {
    //         ErrorLog("tiles are not an Area");
    //     }
    //     
    //     bool xForward = startCorner.x <= endCorner.x;
    //     bool zForward = startCorner.z <= endCorner.z;
    //
    //     List<EmergeTile> tiles = new List<EmergeTile>();
    //
    //     if (xForward) {
    //         for (int x = startCorner.x + 1; x < endCorner.x; x++) {
    //             if (zForward) {
    //                 for (int z = startCorner.z + 1; z < endCorner.z; z++) {
    //                     tiles.Add(_gridArray[x, z] as EmergeTile);
    //                 }
    //             } else {
    //                 for (int z = startCorner.z - 1; z > endCorner.z; z--) {
    //                     tiles.Add(_gridArray[x, z] as EmergeTile);
    //                 }
    //             }
    //         }
    //     } else {
    //         for (int x = startCorner.x - 1; x > endCorner.x; x--) {
    //             if (zForward) {
    //                 for (int z = startCorner.z + 1; z < endCorner.z; z++) {
    //                     tiles.Add(_gridArray[x, z] as EmergeTile);
    //                 }
    //             } else {
    //                 for (int z = startCorner.z - 1; z > endCorner.z; z--) {
    //                     tiles.Add(_gridArray[x, z] as EmergeTile);
    //                 }
    //             }
    //         }
    //     }
    //
    //     return tiles;
    // }

   

    public void GetGridPosition(Vector3 worldPos, out int x, out int z) {
        x = Mathf.FloorToInt((worldPos - _originPos).x / _cellSize);
        z = Mathf.FloorToInt((worldPos - _originPos).z / _cellSize);
    }

    public Vector3 GetWorldPositionOrigin(int x, int z) {
        return new Vector3(x, 0, z) * _cellSize + _originPos;
    }
    
    public Vector3 GetWorldPositionCenter(int x, int z) {
        return new Vector3(x, 0, z) * _cellSize + _originPos + centerOffset;
    }
    
    // public Vector3 GetWorldPositionCenter(EmergeTile tile) {
    //     return GetWorldPositionCenter(tile.x, tile.z);
    // }

    public float GetCellSize() {
        return _cellSize;
    }

    public int GetWidth() {
        return _width;
    }
    public int GetHeight() {
        return _height;
    }
}