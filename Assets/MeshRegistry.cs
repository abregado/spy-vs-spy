using System;
using System.Collections;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshRegistry : MonoBehaviour {
    public Mesh[] itemMeshes;
    private Dictionary<G.ItemType, Mesh> _items;
    public GameObject[] furniturePrefabs;
    private Dictionary<G.FurnitureTypes, GameObject> _furniture;

    

    public void Init() {
        _items = new Dictionary<G.ItemType, Mesh>();
        for (int i = 1; i < itemMeshes.Length; i++) {
            _items.Add((G.ItemType) i,itemMeshes[i-1]);
        }

        _furniture = new Dictionary<G.FurnitureTypes, GameObject>();
        for (int i = 1; i < furniturePrefabs.Length; i++) {
            _furniture.Add((G.FurnitureTypes) i, furniturePrefabs[i-1]);
        }
    }

    public GameObject GetFurniturePrefab(G.FurnitureTypes type) {
        if (_furniture.ContainsKey(type)) {
            return _furniture[type];
        }
        
        return null;
    }

    public GameObject GetRandomFurniture() {
        return furniturePrefabs[Random.Range(0, furniturePrefabs.Length)];
    }

    public Mesh GetMesh(G.ItemType itemType) {
        if (_items.ContainsKey(itemType)) {
            return _items[itemType];
        }
        
        return null;
    }
}
