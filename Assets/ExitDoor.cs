
using UnityEngine;

public class ExitDoor: MonoBehaviour, IInteractable {
    public bool interactable => true;
    public bool correctExit;
    public bool unlocked;

    private BriefcaseHandler _briefcase;

    void Awake() {
        _briefcase = FindObjectOfType<BriefcaseHandler>();
    }
    
    public void OnInteract(Spy spy) {
        if (spy.inventory == G.ItemType.Briefcase && _briefcase.IsComplete()) {
            Debug.Log("You Win!");
            spy.GotoWinRoom();
        }
    }
}
