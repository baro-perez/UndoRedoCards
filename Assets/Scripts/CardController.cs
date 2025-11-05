using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AlvaroPerez.UndoRedoCards
{

    public class CardController : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform nextCardAnchor;

        public RectTransform NextCardAnchor => nextCardAnchor;

        public IReadonlyCardId CardId { get; private set; }
        public StackController Stack { get; set; }
        public bool IsDragging { get; set; }

        private Canvas canvas;
        private Canvas Canvas => canvas != null ? canvas : canvas = FindCanvas();

        private RectTransform rectTransform;
        private RectTransform RectTransform => rectTransform != null ? RectTransform : transform as RectTransform;

        private Transform originalParent;
        private Vector3 originalPosition;

        private Canvas FindCanvas()
        {
            for(var t = transform.parent; t != null; t = t.parent)
            {
                if (t.TryGetComponent<Canvas>(out var canvas))
                {
                    return canvas;
                }
            }

            return null;
        }

        public void Setup(
            IReadonlyCardId cardId,
            Sprite sprite)
        {
            CardId = cardId;

            if (image != null)
            {
                image.sprite = sprite;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Stack.IsTopCard(this))
            {
                return;
            }

            IsDragging = true;
            originalParent = transform.parent;

            // bring to front while dragging
            originalPosition = transform.position;
            transform.SetParent(Canvas.transform, worldPositionStays: true);

            // allow drop targets to detect drop
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            RectTransform.anchoredPosition += eventData.delta / Canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!IsDragging)
            {
                return;
            }

            IsDragging = false;

            // Raycast to see if dropped on another card
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Reset raycastTarget
            image.raycastTarget = true;

            StackController stackDropped = null;
            foreach (var result in results)
            {
                var droppedGameObject = result.gameObject;
                if (droppedGameObject.TryGetComponent<CardController>(out var cardController))
                {
                    stackDropped = cardController.Stack;
                    break;
                }
                else if (droppedGameObject.TryGetComponent<StackController>(out var stackController))
                {
                    stackDropped = stackController;
                    break;
                }
            }

            transform.SetParent(originalParent, worldPositionStays: true);

            if (stackDropped != null && stackDropped != Stack)
            {
                // Dropped
                Stack.PopCard();
                stackDropped.PushCard(this);
            }
            else
            {
                // If not dropped on another card, reset card
                transform.position = originalPosition;
            }
        }
    }
}
