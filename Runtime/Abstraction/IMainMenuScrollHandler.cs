using System;
using UnityEngine.EventSystems;

namespace GameWarriors.UIDomain.Abstraction
{
    public interface IMainMenuScrollHandler 
    {
        event Action OnScrollBegin;
        event Action<int> OnDragEnd;
        event Action<int> OnMoveToContentCalled;
        float NormalPosition { get; }
        void MoveToContent(int contentIndex);
        void UpdateDrag(PointerEventData eventData);
        void UpdateOnEndDrag(PointerEventData eventData);
        void UpdateOnScroll(PointerEventData eventData);
        void InitializePotentialDrag(PointerEventData eventData);
        void BeginDrag(PointerEventData eventData);
    }
}
