using System;

namespace GameWarriors.UIDomain.Abstraction
{
    internal struct ScreenStackItem
    {
        internal IScreenItem ScreenItem { get; }
        internal EScreenState ScreenState { get; private set; }
        internal string ScreenName => ScreenItem.ScreenName;
        public bool HasBlackScreen => ScreenItem.HasBlackScreen;
        public float OpenAnimationDuration => ScreenItem.OpenAnimationDuration;

        public bool IsValid => ScreenItem != null;

        public bool CanCallShow => ScreenState != EScreenState.Open;

        internal ScreenStackItem(IScreenItem screenItem)
        {
            ScreenState = EScreenState.Open;
            ScreenItem = screenItem;
        }

        internal void OnClose(float delay)
        {
            ScreenState = EScreenState.Close;
            ScreenItem.OnClose(delay);
        }

        internal void OnHide(bool deactivate, float openAnimationDuration)
        {
            ScreenState = EScreenState.Hide;
            ScreenItem.OnHide(deactivate, openAnimationDuration);
        }

        internal void OnShow(bool showAnimation)
        {
            ScreenState = EScreenState.Open;
            ScreenItem.OnShow(showAnimation: showAnimation);
        }
    }
}