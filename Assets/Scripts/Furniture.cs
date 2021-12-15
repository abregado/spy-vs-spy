using DG.Tweening;
using UnityEngine;

public class Furniture: MonoBehaviour, IInteractable {
    public bool interactable { get; }

    public MeshRegistry.ItemType inventory;
    public Room myRoom;

    private DOTweenAnimation _animation;
    private BriefcaseHandler _briefcase;

    void Awake() {
        _animation = transform.Find("View").GetComponent<DOTweenAnimation>();
        _briefcase = FindObjectOfType<BriefcaseHandler>();
    }
    
    public void OnInteract(Spy spy) {
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
}
