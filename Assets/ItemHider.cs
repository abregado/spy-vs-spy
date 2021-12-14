using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemHider : MonoBehaviour {

    public MeshRegistry.ItemType[] objectsToHide;
    private Furniture[] _hidingLocations;
    
    private void Awake() {
        _hidingLocations = FindObjectsOfType<Furniture>();

        foreach (Furniture location in _hidingLocations) {
            location.inventory = MeshRegistry.ItemType.None;
        }

        List<Furniture> unusedLocations = _hidingLocations.ToList();
        foreach (MeshRegistry.ItemType item in objectsToHide) {
            if (unusedLocations.Count > 0) {
                Furniture randomLocation = unusedLocations[Random.Range(0, unusedLocations.Count - 1)];
                randomLocation.inventory = item;
                unusedLocations.Remove(randomLocation);
            }
        }
    }
}
