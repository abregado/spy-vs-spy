using DG.Tweening;
using UnityEngine;

public class Furniture: MonoBehaviour, IInteractable, ICanBeTrapped {
    public bool interactable => true;
    public bool trappable => true;

    public bool isTrapped;

    public G.ItemType inventory;
    public Room myRoom;

    private DOTweenAnimation _animation;
    private BriefcaseHandler _briefcase;
    private BriefcaseUI _ui;

    void Awake() {
        _animation = transform.Find("View").GetComponent<DOTweenAnimation>();
        _briefcase = FindObjectOfType<BriefcaseHandler>();
        _ui = FindObjectOfType<BriefcaseUI>();
        myRoom = transform.parent.GetComponent<Room>();
    }
    
    public virtual void OnInteract(Spy spy) {
        if (isTrapped) {
            spy.KillByTrap(myRoom);
            isTrapped = false;
            return;
        }
        
        if (spy.inventory == G.ItemType.Briefcase && (inventory == G.ItemType.None | inventory == G.ItemType.Briefcase) == false && _briefcase.CheckNeedsItem(inventory)) {
            _briefcase.PutInItem(inventory);
            inventory = G.ItemType.None;
        }
        else {
            (inventory, spy.inventory) = (spy.inventory, inventory);
            if (spy.inventory == G.ItemType.Briefcase) {
                _ui.SetPanelVisiblity(true);
            }
        }
        
        spy.SetInventoryMesh();
        PlaySearchAnimation();
    }

    private void PlaySearchAnimation() {
        _animation.DORestart();
        _animation.DOPlay();
    }
    
    public void OnTrapSet(Spy spy) {
        
        if (isTrapped) {
            spy.KillByTrap(myRoom);
            isTrapped = false;
            return;
        }
        
        if (isTrapped == false) {
            Debug.Log("Setting Trap");
            isTrapped = true;
            PlaySearchAnimation();
        }
    }
}
