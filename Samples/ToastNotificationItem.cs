using GameWarriors.UIDomain.Abstraction;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    public class ToastNotificationItem : MonoBehaviour, IToastItem
    {
        [SerializeField] private TextMeshProUGUI _contextLable;

        [SerializeField] private Animation _animation;

        private Transform _transform;
        public bool Activation { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
        public Vector2 Position { get => _transform.localPosition; set => _transform.localPosition = value; }

        public void Initialization()
        {
            _transform = transform;
            Activation = false;
        }

        public void SetData(string context)
        {
            _animation.Stop();
            _contextLable.text = context;
            _animation.Play();
            _transform.SetAsLastSibling();
            Activation = true;
        }
    }
}