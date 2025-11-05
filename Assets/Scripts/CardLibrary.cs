using System;
using UnityEngine;

namespace AlvaroPerez.UndoRedoCards
{
    [CreateAssetMenu(
        fileName = nameof(CardLibrary),
        menuName = nameof(CardLibrary))]
    public class CardLibrary : ScriptableObject
    {
        [SerializeField] private Sprite[] diamonds = Array.Empty<Sprite>();
        [SerializeField] private Sprite[] clubs = Array.Empty<Sprite>();
        [SerializeField] private Sprite[] spades = Array.Empty<Sprite>();
        [SerializeField] private Sprite[] hearts = Array.Empty<Sprite>();

        public Sprite GetSprite(IReadonlyCardId cardId)
        {
            return GetSprite(cardId.Suit, cardId.Number);
        }

        public Sprite GetSprite(Suit suit, int number)
        {
            var suitLibrary = suit switch
            {
                Suit.Clubs => clubs,
                Suit.Diamonds => diamonds,
                Suit.Spades => spades,
                Suit.Hearts => hearts,
                _ => throw new ArgumentOutOfRangeException(),
            };

            return suitLibrary[number - 1];
        }
    }
}
