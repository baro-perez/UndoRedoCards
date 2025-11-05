using System;
using UnityEngine;

namespace AlvaroPerez.UndoRedoCards
{
    [Serializable]
    public class CardIdArray
    {
        [SerializeField] private CardId[] cards;

        public CardId this[int i] => cards[i];
        public int Length => cards.Length;
    }
}
