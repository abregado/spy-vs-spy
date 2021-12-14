public interface IInteractable {
    public bool interactable { get; }
    public void OnInteract(Spy spy);
}
