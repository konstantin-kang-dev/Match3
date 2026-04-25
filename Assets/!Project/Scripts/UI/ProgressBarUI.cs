using DG.Tweening;
using UnityEngine;

namespace Game.UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _fill;
        [SerializeField] private ParticleSystem _updateVfx;

        [Header("Animation")]
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private Ease _ease = Ease.OutCubic;

        private float _currentValue;
        private Tween _tween;

        private void Awake()
        {
            ApplyFill(0f);
        }

        public void SetValue(float normalized, bool animated = true)
        {
            normalized = Mathf.Clamp01(normalized);

            _tween?.Kill();

            if (animated)
            {
                _tween = DOTween.To(() => _currentValue, ApplyFill, normalized, _animationDuration)
                    .SetEase(_ease)
                    .SetLink(gameObject);
            }
            else
            {
                ApplyFill(normalized);
            }

            if (_updateVfx != null)
                _updateVfx.Play();
        }

        private void ApplyFill(float value)
        {
            _currentValue = value;
            _fill.anchorMax = new Vector2(value, _fill.anchorMax.y);
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}

