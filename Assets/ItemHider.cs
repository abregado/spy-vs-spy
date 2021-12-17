using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemHider : MonoBehaviour {

    public G.ItemType[] objectsToHide;
    private Furniture[] _hidingLocations;
    
    public void Init() {
        LevelGenerator genny = FindObjectOfType<LevelGenerator>();
        List<Room> unusedRooms = new List<Room>();
        
        foreach (Furniture location in genny.placeableFurniture) {
            location.inventory = G.ItemType.None;
        }

        foreach (Room room in genny.rooms) {
            if (room.HasAnyFurnitureEmpty()) {
                unusedRooms.Add(room);
            }
        }
        
        foreach (G.ItemType item in objectsToHide) {
            if (unusedRooms.Count > 0) {
                try_again:
                Room randomRoom = unusedRooms[Random.Range(0, unusedRooms.Count - 1)];
                if (randomRoom.HideItem(item)) {
                    unusedRooms.Remove(randomRoom);    
                }
                else {
                    goto try_again;
                }
            }
        }
    }

    public void HideItemAnywhere(G.ItemType item) {
        _hidingLocations = FindObjectsOfType<Furniture>();

        List<Furniture> emptyRoomFurni = new List<Furniture>();
        
        foreach (Furniture furniture in _hidingLocations) {
            if (furniture.inventory == G.ItemType.None) {
                emptyRoomFurni.Add(furniture);
            }
        }

        emptyRoomFurni[Random.Range(0, emptyRoomFurni.Count)].inventory = item;
    }
}
