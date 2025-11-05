using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlvaroPerez.UndoRedoCards
{
    public class StackController : MonoBehaviour
    {
        [SerializeField] private RectTransform anchor;

        private Stack<CardController> cards = new();


        #region OnCardPushed
        public readonly struct CardPushedEventData
        {
            public readonly CardController Card;
            public readonly StackController PrevStack;

            public CardPushedEventData(CardController card, StackController prevStack)
            {
                Card = card;
                this.PrevStack = prevStack;
            }
        };
        public delegate void CardPushedEventHandler(
            StackController source, CardPushedEventData eventData);
        public event CardPushedEventHandler OnCardPushed;
        #endregion

        #region OnCardPopped
        public readonly struct CardPoppedEventData
        {
            public readonly CardController Card;

            public CardPoppedEventData(CardController card)
            {
                Card = card;
            }
        };
        public delegate void CardPoppedEventHandler(
            StackController source, CardPoppedEventData eventData);
        public event CardPoppedEventHandler OnCardPopped;
        #endregion

        public void PushCard(CardController card)
        {
            RectTransform anchor;
            if (cards.Count > 0)
            {
                var topCard = cards.Peek();
                anchor = topCard.NextCardAnchor;
            }
            else
            {
                anchor = this.anchor;
            }
            card.transform.position = anchor.position;
            var prevStack = card.Stack;
            card.Stack = this;
            cards.Push(card);
            OnCardPushed?.Invoke(this, new(card, prevStack));
        }

        public CardController PopCard()
        {
            var result = cards.TryPop(out var card) ? card : null;
            if (result != null)
            {
                OnCardPopped?.Invoke(this, new(result));
            }
            return result;
        }

        public bool IsTopCard(CardController card)
        {
            return cards.TryPeek(out var peek) && card == peek;
        }
    }
}
