using UnityEngine;

public class RoomMeshAndMatRandomizer : MonoBehaviour {
    public Mesh[] apocFloorMeshes;
    public Mesh[] apocWallMeshes;
    public Material[] apocMats;
    
    public Mesh[] militaryFloorMeshes;
    public Mesh[] militaryWallMeshes;
    public Material[] militaryMats;

    public Mesh[] townFloorMeshes;
    public Mesh[] townWallMeshes;
    public Material[] townMats;
    
    
    void Awake() {
        int randomFloorType = Random.Range(0, 27);
        Mesh floorMesh;
        Material floorMaterial;
        if (randomFloorType < 20) {
            floorMesh = apocFloorMeshes[Random.Range(0, apocFloorMeshes.Length - 1)];
            floorMaterial = apocMats[Random.Range(0, apocMats.Length - 1)];
        } else if (randomFloorType < 24) {
            floorMesh = militaryFloorMeshes[Random.Range(0, militaryFloorMeshes.Length - 1)];
            floorMaterial = militaryMats[Random.Range(0, militaryMats.Length - 1)];
        } else {
            floorMesh = townFloorMeshes[Random.Range(0, townFloorMeshes.Length - 1)];
            floorMaterial = townMats[Random.Range(0, townMats.Length - 1)];
        }
        Transform floor = transform.Find("Floor");
        floor.GetComponent<MeshFilter>().mesh = floorMesh;
        floor.GetComponent<MeshRenderer>().material = floorMaterial;
        
        int randomWallType = Random.Range(0, 15);
        Mesh wallMesh;
        Material wallMaterial;
        if (randomWallType < 8) {
            wallMesh = apocWallMeshes[Random.Range(0, apocWallMeshes.Length - 1)];
            wallMaterial = apocMats[Random.Range(0, apocMats.Length - 1)];
        } else if (randomWallType < 11) {
            wallMesh = militaryWallMeshes[Random.Range(0, militaryWallMeshes.Length - 1)];
            wallMaterial = militaryMats[Random.Range(0, militaryMats.Length - 1)];
        }
        else {
            wallMesh = townWallMeshes[Random.Range(0, townWallMeshes.Length - 1)];
            wallMaterial = townMats[Random.Range(0, townMats.Length - 1)];
        }
        Transform wall1 = transform.Find("BackWall");
        Transform wall2 = transform.Find("LeftWall");
        Transform wall3 = transform.Find("RightWall");
        wall1.GetComponent<MeshFilter>().mesh = wallMesh;
        wall2.GetComponent<MeshFilter>().mesh = wallMesh;
        wall3.GetComponent<MeshFilter>().mesh = wallMesh;
        wall1.GetComponent<MeshRenderer>().material = wallMaterial;
        wall2.GetComponent<MeshRenderer>().material = wallMaterial;
        wall3.GetComponent<MeshRenderer>().material = wallMaterial;

    }


}
