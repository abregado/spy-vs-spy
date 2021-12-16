using UnityEngine;

public class GameManager : MonoBehaviour {

    public LevelGenerator levelGen;
    public RoomCameraSystem roomCameraSystem;
    public MeshRegistry meshReg;
    public BriefcaseHandler briefCase;
    public ItemHider itemHider;
    
    private void Start() {
        meshReg.Init();
        briefCase.Init();
        levelGen.BuildMap();
        roomCameraSystem.Init();
        itemHider.Init();
    }
    
}