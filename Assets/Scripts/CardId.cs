using System;
using UnityEngine;

namespace AlvaroPerez.UndoRedoCards
{
    [Serializable]
    public class CardId : IReadonlyCardId
    {
        [SerializeField] private Suit suit;
        [SerializeField] private int number;

        public Suit Suit => suit;
        public int Number => number;
    }
}
