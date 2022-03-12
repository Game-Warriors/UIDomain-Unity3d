using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameWarriors.UIDomain.Core
{
    public class SpriteBlackPanel : MonoBehaviour
    {
        [SerializeField] private float _maxAlpha = 0.75f;
        private SpriteRenderer _blackSprite;
        private float _targetAlpha;
        private float _fadeSpeed;
        private Action _fadeOutDone;


        public bool Activation { set => gameObject.SetActive(value); get => gameObject.activeSelf; }

        public void Initialization(string name)
        {
            this.name = name;
            _blackSprite = GetComponent<SpriteRenderer>();
            Color color = _blackSprite.color;
            color.a = 0;
            gameObject.SetActive(false);
            _blackSprite.color = color;
        }

        public void DisableScreen()
        {
            _fadeSpeed = 0;
            Color color = _blackSprite.color;
            color.a = 0;
            gameObject.SetActive(false);
            _blackSprite.color = color;
        }

        public void ShowIn(float second)
        {
            _targetAlpha = _maxAlpha;
            _fadeSpeed = (_maxAlpha - _blackSprite.color.a) / second;
            _fadeOutDone?.Invoke();
            _fadeOutDone = null;
            _fadeSpeed = Mathf.Max(0.01f, _fadeSpeed);
            gameObject.SetActive(true);
        }

        public void FadeOut(float second, Action fadeOutDone)
        {
            _targetAlpha = 0;
            _fadeSpeed = (_blackSprite.color.a) / second;
            _fadeOutDone = fadeOutDone;
            _fadeSpeed = Mathf.Max(0.01f, _fadeSpeed);
        }

        private void Update()
        {
            if (_fadeSpeed > 0)
            {
                Color color = _blackSprite.color;
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
                _blackSprite.color = color;
            }
        }

        public void SetSortingOrder(int sortOrder)
        {
            _blackSprite.sortingOrder = sortOrder;
        }

        public void SetAlpha(float alpha)
        {
            Color color = _blackSprite.color;
            color.a = alpha;
            _blackSprite.color = color;
        }
    }
}