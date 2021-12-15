using DG.Tweening;
using UnityEngine;

public class Furniture: MonoBehaviour, IInteractable, ICanBeTrapped {
    public bool interactable => true;
    public bool trappable => true;

    public bool isTrapped;

    public MeshRegistry.ItemType inventory;
    public Room myRoom;

    private DOTweenAnimation _animation;
    private BriefcaseHandler _briefcase;

    void Awake() {
        _animation = transform.Find("View").GetComponent<DOTweenAnimation>();
        _briefcase = FindObjectOfType<BriefcaseHandler>();
        myRoom = transform.parent.GetComponent<Room>();
    }
    
    public void OnInteract(Spy spy) {
        if (isTrapped) {
            spy.KillByTrap(myRoom);
            isTrapped = false;
            return;
        }
        
        if (spy.inventory == MeshRegistry.ItemType.Briefcase && (inventory == MeshRegistry.ItemType.None | inventory == MeshRegistry.ItemType.Briefcase) == false && _briefcase.CheckNeedsItem(inventory)) {
            _briefcase.PutInItem(inventory);
            inventory = MeshRegistry.ItemType.None;
        }
        else {
            (inventory, spy.inventory) = (spy.inventory, inventory);    
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
