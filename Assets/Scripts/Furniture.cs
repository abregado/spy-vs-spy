using DG.Tweening;
using UnityEngine;

public class Furniture: MonoBehaviour, IInteractable {
    public bool interactable { get; }

    public int inventory;

    private DOTweenAnimation _animation;

    void Awake() {
        _animation = transform.Find("View").GetComponent<DOTweenAnimation>();
    }
    
    
    public void OnInteract(Spy spy) {
        (inventory, spy.inventory) = (spy.inventory, inventory);

        spy.SetInventoryMesh();
        PlaySearchAnimation();
    }

    private void PlaySearchAnimation() {
        _animation.DOPlay();
    }

    public void ResetAnimation() {
        
    }
}
