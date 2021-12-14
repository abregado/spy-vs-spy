
using UnityEngine;

public class ExitDoor: MonoBehaviour, IInteractable {
    public bool interactable { get; }

    private BriefcaseHandler _briefcase;

    void Awake() {
        _briefcase = FindObjectOfType<BriefcaseHandler>();
    }
    
    public void OnInteract(Spy spy) {
        if (spy.inventory == MeshRegistry.ItemType.Briefcase && _briefcase.IsComplete()) {
            Debug.Log("You Win!");
        }
    }
}
