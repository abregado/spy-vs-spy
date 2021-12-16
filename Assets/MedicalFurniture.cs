
using DG.Tweening;
using UnityEngine;

public class MedicalFurniture : MonoBehaviour, IInteractable
{
    public bool interactable => true;
    
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
    
    public void OnInteract(Spy spy) {
        spy.healthPoints = 30;
        PlaySearchAnimation();
    }

    private void PlaySearchAnimation() {
        _animation.DORestart();
        _animation.DOPlay();
    }
}
