using UnityEngine;

public class GameManager : MonoBehaviour {

    public LevelGenerator levelGen;
    public RoomCameraSystem roomCameraSystem;
    public MeshRegistry meshReg;
    public BriefcaseHandler briefCase;
    public ItemHider itemHider;
    public SpyHandler spyHandler;
    public GridVisualizer gridViz;
    
    private void Start() {
        meshReg.Init();
        briefCase.Init();
        levelGen.Init(this);
        levelGen.BuildMap();
        roomCameraSystem.Init();
        itemHider.Init();
        spyHandler.Init();
        gridViz.SetGrid(levelGen.grid);
        gridViz.DrawMarker();
    }
    
}