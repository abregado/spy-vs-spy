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
    private float nextTrappedAnimationCycle;

    public void Init(Room room) {
        _animation = transform.Find("View").GetComponent<DOTweenAnimation>();
        _briefcase = FindObjectOfType<BriefcaseHandler>();
        _ui = FindObjectOfType<BriefcaseUI>();
        myRoom = room;
    }

    void Update() {
        if (isTrapped && Time.time > nextTrappedAnimationCycle) {
            PlayTrappedAnimation();
            nextTrappedAnimationCycle =
                Time.time + Random.Range(G.FURNITURE_MIN_WIGGLE_TIME, G.FURNITURE_MAX_WIGGLE_TIME);
        }
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
        _animation.DORestartById("search");
        _animation.DOPlayById("search");
    }

    private void PlayTrappedAnimation() {
        _animation.DORestartById("trapped");
        _animation.DOPlayById("trapped");
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
            nextTrappedAnimationCycle =
                Time.time + Random.Range(G.FURNITURE_MIN_WIGGLE_TIME, G.FURNITURE_MAX_WIGGLE_TIME);
            PlaySearchAnimation();
        }
    }
}
