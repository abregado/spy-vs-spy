public interface ICanBeTrapped {
    bool trappable { get; }

    void OnTrapSet(Spy spy);
}
