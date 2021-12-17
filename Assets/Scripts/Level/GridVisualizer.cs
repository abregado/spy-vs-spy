using System.Collections.Generic;
using System.Linq;
using CodeMonkey.Utils;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;

public class GridVisualizer : MonoBehaviour {
        
    private readonly Vector3 spriteRot = new Vector3(-90, 0,0);
    private readonly Vector3 textRot = new Vector3(90, 0,0);
    private Grid<RoomTile> grid;
    private Dictionary<RoomTile, SpriteRenderer> tileVisuals;
    private Dictionary<RoomTile, TextMeshPro> coordVisuals;
    
    public Transform gridSprites;
    // public Transform gridTexts;
    public Sprite gridSprite;

    
    private void OnTileChangeEvent(RoomTile t) {
        if (tileVisuals.Count == 0) {
            return;
        }
        if (tileVisuals.TryGetValue(t, out SpriteRenderer renderer)) {
            DrawTile(t, renderer);
        } else {
            Debug.Log("OnTileChangeEvent visuals not found for " + t.gridPosition.x + ", " + t.gridPosition.y);
        }
    }

    public void SetGrid(Grid<RoomTile> grid) {
        this.grid = grid;
        tileVisuals = new Dictionary<RoomTile, SpriteRenderer>();
        coordVisuals = new Dictionary<RoomTile, TextMeshPro>();
    }

    public void RemoveGrid() {
        this.grid = null;
        RemoveMarker();
    }

    public void RemoveMarker() {
        foreach (KeyValuePair<RoomTile,SpriteRenderer> kvp in tileVisuals) {
            SafeDestroyGameObjectWithComponent(kvp.Value);
        }
        tileVisuals.Clear();
        
        foreach (KeyValuePair<RoomTile,TextMeshPro> kvp in coordVisuals) {
            SafeDestroyGameObjectWithComponent(kvp.Value);
        }
        coordVisuals.Clear();
        
        // EventManager.OnTileChanged -= OnTileChangeEvent;
    }

    private void OnApplicationQuit() {
        // EventManager.OnTileChanged -= OnTileChangeEvent;
    }

    public void DrawMarker() {
         if (grid == null) {
            Debug.Log("DrawMarker -> Grid not Loaded");
            return;
         }
         
         // EventManager.OnTileChanged += OnTileChangeEvent;
         for (int x = 0; x < grid._gridArray.GetLength(0); x++) {
             for (int z = 0; z < grid._gridArray.GetLength(1); z++) {

                 RoomTile tile = grid._gridArray[x, z];
                 if (tile == null) {
                     Debug.LogError("Grid is not initialized correctly");
                     return;
                 }

                 if (tile.room == null) {
                     continue;
                 }
                 Color c = Color.white;
                 SpriteRenderer renderer = UtilsClass.CreateWorldSprite(gridSprites, "Dot" + tile.gridPosition.x + "-" + tile.gridPosition.y, 
                     gridSprite, grid.GetWorldPositionCenter(tile.gridPosition.x, tile.gridPosition.y), 
                     new Vector3(grid.GetCellSize(), grid.GetCellSize(), 0), 1, c).GetComponent<SpriteRenderer>();
                 renderer.transform.Rotate(spriteRot);
                 renderer.gameObject.layer = 11;
                 tileVisuals.Add(tile, renderer);
                 DrawTile(tile, renderer);
             }
         }
        
         // for (int x = 0; x < grid._gridArray.GetLength(0); x++) {
         //     for (int z = 0; z < grid._gridArray.GetLength(1); z++) {
         //
         //         RoomTile tile = grid._gridArray[x, z];
         //         if (tile == null) {
         //             Debug.LogError("Grid is not initialized correctly");
         //             return;
         //         }
         //         Color c = Color.black;
         //         string text = "" + x + "-" + z;
         //         TextAlignmentOptions alignment = TextAlignmentOptions.Bottom;
         //         Vector2 rectSize = new Vector2(grid.GetCellSize(), grid.GetCellSize());
         //         TextMeshPro textMesh = UtilsClass.CreateWorldTMPText(text, rectSize, text, gridTexts, 
         //             grid.GetWorldPositionCenter(tile.gridPosition.x, tile.gridPosition.y), 2, c, alignment);
         //         textMesh.transform.Rotate(textRot);
         //         coordVisuals.Add(tile, textMesh);
         //         // DrawTile(tile, renderer);
         //     }
         // }
    }

    private void DrawTile(RoomTile tile, SpriteRenderer spriteRenderer) {
        Color c = Color.white;
        spriteRenderer.color = c;
    }
    
    public static T[] GetTypesFromContainersAndChildren<T>(Transform[] containers) {
        List<T> collectedTypes = new List<T>();
        foreach (Transform container in containers) {
            GetAllTypes(container, collectedTypes);
        }
        return collectedTypes.Where(c => c != null).ToArray();
    }
    
    public static T[] GetTypesFromContainerAndChildren<T>(Transform container) {
        List<T> collectedTypes = new List<T>();
        GetAllTypes(container, collectedTypes);
        return collectedTypes.Where(c => c != null).ToArray();
    }
    
    private static void GetAllTypes<T>(Transform transform, List<T> collection) {
        for (int i = 0; i < transform.childCount; i++) {
            GetAllTypes<T>(transform.GetChild(i), collection);
        }
        collection.Add(transform.GetComponent<T>());
    }
    
    // Collects all T from the containers children, returns an array<T>
    public static T[] getTypesFromContainers<T>(Transform[] containers) {
        int containerCount = 0;
        for (int i = 0; i < containers.Length; i++) {
            containerCount += containers[i].transform.childCount;
        }

        T[] combined = new T[containerCount];
        int index = 0;
        for (int i = 0; i < containers.Length; i++) {
            Transform root = containers[i].transform;
            for (int k = 0; k < root.childCount; k++) {
                T item = root.transform.GetChild(k).GetComponent<T>();
                combined[index] = item;
                index++;
            }
        }
        return combined.Where(c => c != null).ToArray();
    }

    public static T[] GetTypesFromContainer<T>(Transform container) {
        T[] types = new T[container.childCount];
        
        for (int k = 0; k < container.childCount; k++) {
            T item = container.transform.GetChild(k).GetComponent<T>();
            types[k] = item;
        }
        return types.Where(c => c != null).ToArray();
    }

    public static T SafeDestroyGameObjectWithComponent<T>(T component) where T : Component {
        if (component != null) {
            SafeDestroyObject(component.gameObject);
        }
        return null;
    }
    private static T SafeDestroyObject<T>(T obj) where T : Object
    {
        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);

        return null;
    }
}