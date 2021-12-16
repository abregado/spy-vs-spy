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
        _hidingLocations = FindObjectsOfType<Furniture>();

        foreach (Furniture location in _hidingLocations) {
            location.inventory = G.ItemType.None;
        }

        List<Furniture> unusedLocations = _hidingLocations.ToList();
        foreach (G.ItemType item in objectsToHide) {
            if (unusedLocations.Count > 0) {
                Furniture randomLocation = unusedLocations[Random.Range(0, unusedLocations.Count - 1)];
                randomLocation.inventory = item;
                unusedLocations.Remove(randomLocation);
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
