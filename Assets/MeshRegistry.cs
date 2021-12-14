using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MeshRegistry : MonoBehaviour {
    public Mesh[] itemMeshes;
    private Dictionary<ItemType, Mesh> _items;

    public enum ItemType {
        None,
        Briefcase,
        Rope,
        Disguise,
        Money,
        Bomb,
    }

    void Awake() {
        _items = new Dictionary<ItemType, Mesh>();

        for (int i = 1; i < itemMeshes.Length+1; i++) {
            _items.Add((ItemType) i,itemMeshes[i-1]);
        }
    }
    
    

    public Mesh GetMesh(ItemType itemType) {
        if (_items.ContainsKey(itemType)) {
            return _items[itemType];
        }
        
        return null;
    }
}
