using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using R3;
using DG.Tweening;
using Taki.Utility;

namespace Taki.Main.View
{
    [Serializable]
    public struct GraphicColorEntry
    {
        public Graphic Graphic;
        public Color HoverColor;
    }

    public sealed class PointerColorAnimator : PointerEventHandler
    {
        [SerializeField] private List<GraphicColorEntry> _graphicEntries = new();
        [SerializeField] private float _animationDuration = 0.2f;
        [SerializeField] private Ease _easeType = Ease.OutQuad;
        [SerializeField] private bool _ignoreTimeScale = false;

        private Dictionary<Graphic, Color> _originalColors = new();
        private List<Tween> _currentTweens = new();

        protected override void Awake()
        {
            base.Awake();

            InitializeGraphicEntries(_graphicEntries);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _currentTweens.ForEach(tween => tween?.Kill());
        }

        public void SetGraphicEntries(List<GraphicColorEntry> newGraphicEntries, bool append = false)
        {
            _currentTweens.ForEach(tween => tween?.Kill());
            _currentTweens.Clear();

            if (!append)
            {
                _graphicEntries.Clear();
                _originalColors.Clear();
            }

            _graphicEntries.AddRange(newGraphicEntries);
            InitializeGraphicEntries(newGraphicEntries);
        }

        private void InitializeGraphicEntries(List<GraphicColorEntry> entries)
        {
            entries
                .Where(entry => entry.Graphic != null)
                .ToList()
                .ForEach(entry => _originalColors[entry.Graphic] = entry.Graphic.color);
        }

        protected override void OnClicked()
        {
        }

        protected override void OnPointerEntered()
        {
            _currentTweens.ForEach(tween => tween?.Kill());
            _currentTweens.Clear();

            _graphicEntries
                .Where(entry => entry.Graphic != null && _originalColors.ContainsKey(entry.Graphic))
                .ToList()
                .ForEach(entry =>
                {
                    Tween newTween = entry.Graphic.DOColor(entry.HoverColor, _animationDuration)
                        .SetEase(_easeType)
                        .SetUpdate(_ignoreTimeScale);
                    _currentTweens.Add(newTween);
                });
        }

        protected override void OnPointerExited()
        {
            _currentTweens.ForEach(tween => tween?.Kill());
            _currentTweens.Clear();

            _graphicEntries
                .Where(entry => entry.Graphic != null && _originalColors.ContainsKey(entry.Graphic))
                .ToList()
                .ForEach(entry =>
                {
                    Tween newTween = entry.Graphic.DOColor(_originalColors[entry.Graphic], _animationDuration)
                        .SetEase(_easeType)
                        .SetUpdate(_ignoreTimeScale);
                    _currentTweens.Add(newTween);
                });
        }
    }
}