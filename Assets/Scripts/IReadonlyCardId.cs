namespace AlvaroPerez.UndoRedoCards
{
    public interface IReadonlyCardId
    {
        Suit Suit { get; }
        int Number { get; }
    }
}
