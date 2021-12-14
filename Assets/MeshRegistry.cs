using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRegistry : MonoBehaviour {
    public Mesh[] itemMeshes;

    public enum itemTypes {
        None,
        Briefcase,
        Key,
        Passport,
        Money,
        Ticket,
    }

    public Mesh GetMesh(itemTypes itemType) {
        int meshIndex = (int) itemType - 1;
        if (itemMeshes[meshIndex] != null) {
            return itemMeshes[meshIndex];
        }
        return null;
    }
}
