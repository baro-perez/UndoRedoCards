using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace AlvaroPerez.UndoRedoCards
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private RectTransform cardParent;
        [SerializeField] private RectTransform stackParent;
        [Space]
        [SerializeField] private StackController stackPrefab;
        [SerializeField] private CardController cardPrefab;
        [SerializeField] private CardLibrary cardLibrary;
        [Space]
        [SerializeField] private CardSetup initSetup;

        private bool recording = false;
        private Stack<CardCommand> history = new();
        private Stack<CardCommand> redoHistory = new();

        public readonly struct CardCommand
        {
            public readonly CardController Card;
            public readonly StackController PrevStack;
            public readonly StackController NextStack;

            public CardCommand(
                CardController card,
                StackController prevStack,
                StackController nextStack)
            {
                Card = card;
                PrevStack = prevStack;
                NextStack = nextStack;
            }
        };

        public int NStacks => initSetup.Length;

        private readonly List<GameObject> createdObjects = new();
        private readonly List<CardController> cards = new();
        private readonly List<StackController> stacks = new();

        private void Start()
        {
            StartCoroutine(Init());
        }

        private void OnDestroy()
        {
            UnsubscribeStacks();
            DestroyObjects();
        }

        private void Update()
        {
            cards.Sort((a, b) =>
                -a.transform.position.y.CompareTo(b.transform.position.y));
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (!card.IsDragging)
                {
                    card.transform.SetSiblingIndex(i);
                }
            }
        }

        public IEnumerator Init()
        {
            CreateStacks();

            // wait one frame for HorizontalLayoutGroup to take effect
            yield return null;
            InitCards();

            recording = true;
        }

        private void CreateStacks()
        {
            for (int i = 0; i < NStacks; i++)
            {
                var stack = Instantiate(
                    stackPrefab.gameObject, stackParent)
                    .GetComponent<StackController>();
                stacks.Add(stack);
                SubscribeStack(stack);
                createdObjects.Add(stack.gameObject);
            }
        }

        private void UnsubscribeStacks()
        {
            if (stacks == null)
            {
                return;
            }

            for (var i = 0; i < stacks.Count; i++)
            {
                UnsubscribeStack(stacks[i]);
            }
        }

        private void SubscribeStack(StackController stackController)
        {
            stackController.OnCardPushed += OnCardPushed;
            stackController.OnCardPopped += OnCardPopped;
        }

        private void UnsubscribeStack(StackController stackController)
        {
            stackController.OnCardPushed -= OnCardPushed;
            stackController.OnCardPopped -= OnCardPopped;
        }

        private void InitCards()
        {
            if (initSetup == null || initSetup.Length == 0)
            {
                return;
            }

            for (var i = 0; i < initSetup.Length; i++)
            {
                var cardList = initSetup[i];
                if (cardList == null ||
                    cardList.Length == 0 ||
                    stacks.Count <= 0 ||
                    stacks[i] == null)
                {
                    continue;
                }
                var stack = stacks[i];

                for (var j = 0; j < cardList.Length; j++)
                {
                    var cardId = cardList[j];
                    if (cardId == null)
                    {
                        continue;
                    }
                    var card = CreateCard(cardId);
                    stack.PushCard(card);
                }
            }
        }

        private CardController CreateCard(CardId cardId)
        {
            var card = Instantiate(cardPrefab.gameObject, cardParent)
                .GetComponent<CardController>();
            card.Setup(cardId, cardLibrary.GetSprite(cardId));
            createdObjects.Add(card.gameObject);
            cards.Add(card);
            card.name = $"{cardId.Number} of {cardId.Suit}";
            return card;
        }

        private void DestroyObjects()
        {
            for (var i = 0; i < createdObjects.Count; i++)
            {
                var obj = createdObjects[i];
                Destroy(obj);
            }

            createdObjects.Clear();
            cards.Clear();
        }

        private void OnCardPushed(StackController source,
            StackController.CardPushedEventData eventData)
        {
            if (!recording)
            {
                return;
            }

            history.Push(new(eventData.Card, eventData.PrevStack, source));
            redoHistory.Clear();
        }

        private void OnCardPopped(StackController source, 
            StackController.CardPoppedEventData eventData)
        {
        }

        public void Undo()
        {
            if (!history.TryPop(out var command))
            {
                Debug.Log("No commands to undo!");
                return;
            }
            redoHistory.Push(command);

            // Don't register new commands duringg undo/redo!
            recording = false;
            UndoCommand(command);
            recording = true;
        }

        public void Redo()
        {
            if (!redoHistory.TryPop(out var command))
            {
                Debug.Log("No commands to redo!");
                return;
            }
            history.Push(command);

            // Don't register new commands duringg undo/redo!
            recording = false;
            RedoCommand(command);
            recording = true;
        }

        private static void UndoCommand(CardCommand command)
        {
            command.PrevStack.PushCard(command.NextStack.PopCard());
        }

        private static void RedoCommand(CardCommand command)
        {
            command.NextStack.PushCard(command.PrevStack.PopCard());
        }
    }
}
