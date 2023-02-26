using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameWarriors.UIDomain.Core
{
    public class ImageBlackPanel : MonoBehaviour
    {
        [Header("Black panel fade in length in second")]
        [SerializeField] private float _fadeInLength = 0.15f;
        [Header("Black panel fade out length in second")]
        [SerializeField] private float _fadeOutLength = 0.15f;
        [SerializeField] private float _maxAlpha = 0.75f;
        [SerializeField] private Image _blackImage;
        private float _targetAlpha;
        private float _fadeSpeed;
        private Action _fadeOutDone;

        public Transform TransformRef { get; private set; }

        public bool Activation
        {
            set => gameObject.SetActive(value);
            get => gameObject.activeSelf;
        }

        public float TargetAlpha
        {
            get => _maxAlpha;
            set => _maxAlpha = value;
        }

        public void Initialization(string name, Transform parent)
        {
            this.name = name;
            _blackImage = GetComponent<Image>();
            TransformRef = transform;
            gameObject.SetActive(false);
            TransformRef.SetParent(parent, false);
            TransformRef.SetAsFirstSibling();
            Color color = _blackImage.color;
            color.a = 0;
            gameObject.SetActive(false);
            _blackImage.color = color;
        }

        public void DisableScreen()
        {
            _fadeSpeed = 0;
            Color color = _blackImage.color;
            color.a = 0;
            gameObject.SetActive(false);
            _blackImage.color = color;
        }

        public void ShowIn()
        {
            float second = _fadeInLength;
            transform.SetSiblingIndex(transform.parent.childCount - 1);
            _targetAlpha = _maxAlpha;
            _fadeSpeed = (_maxAlpha - _blackImage.color.a) / second;
            _fadeOutDone?.Invoke();
            _fadeOutDone = null;
            _fadeSpeed = Mathf.Max(0.01f, _fadeSpeed);
            gameObject.SetActive(true);
        }

        public void FadeOut(Action fadeOutDone)
        {
            float second = _fadeOutLength;
            _targetAlpha = 0;
            _fadeSpeed = (_blackImage.color.a) / second;
            _fadeOutDone = fadeOutDone;
            _fadeSpeed = Mathf.Max(0.01f, _fadeSpeed);
        }

        private void Update()
        {
            if (_fadeSpeed > 0)
            {
                Color color = _blackImage.color;
                if (Mathf.Abs(color.a - _targetAlpha) > 0.01f)
                {
                    color.a = Mathf.MoveTowards(color.a, _targetAlpha, Time.deltaTime * _fadeSpeed);
                }
                else
                {
                    color.a = _targetAlpha;
                    _fadeSpeed = 0;
                    if (_targetAlpha == 0)
                    {
                        _fadeOutDone?.Invoke();
                        _fadeOutDone = null;
                        gameObject.SetActive(false);
                    }
                }

                _blackImage.color = color;
            }
        }
    }
}