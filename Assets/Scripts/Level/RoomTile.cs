using UnityEngine;

public class RoomTile {
    private Grid<RoomTile> grid;
    public Vector2Int gridPosition;

    public Room room;
    
    public RoomTile(Grid<RoomTile> grid, Vector2Int gridPosition) {
        this.gridPosition = gridPosition;
        this.grid = grid;
    }
}