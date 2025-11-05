using UnityEngine;

namespace AlvaroPerez.UndoRedoCards
{
    [CreateAssetMenu(
        fileName = nameof(CardSetup),
        menuName = nameof(CardSetup))]
    public class CardSetup : ScriptableObject
    {
        [SerializeField] private CardIdArray[] cards;

        public CardIdArray this[int i] => cards[i];
        public int Length => cards.Length;
    }
}
