
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
        if (unlocked && spy.inventory == G.ItemType.Briefcase && _briefcase.IsComplete()) {
            Debug.Log("You Win!");
            spy.GotoWinRoom();
        }
    }

    public void SetUnlocked() {
        Debug.Log("Unlocking exit");
        unlocked = true;
        transform.Find("Blinds").gameObject.SetActive(false);
    }
}
