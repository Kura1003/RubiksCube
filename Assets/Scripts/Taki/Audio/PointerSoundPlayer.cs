using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Taki.Utility;
using Taki.Utility.Core;
using UnityEngine;

namespace Taki.Audio
{
    public sealed class PointerSoundPlayer : PointerEventHandler
    {
        private enum PointerEventType
        {
            Enter,
            Exit,
            Click,
        }

        [Serializable]
        private struct SoundSetting
        {
            public PointerEventType EventType;
            public string SoundName;
            [Range(0f, 1f)] public float Volume;
        }

        [SerializeField] private List<SoundSetting> _soundSettings = new();

        protected override void OnPointerEntered()
        {
            PlaySoundsForEventType(PointerEventType.Enter);
        }

        protected override void OnPointerExited()
        {
            PlaySoundsForEventType(PointerEventType.Exit);
        }

        protected override void OnClicked()
        {
            PlaySoundsForEventType(PointerEventType.Click);
        }

        private void PlaySoundsForEventType(PointerEventType eventType)
        {
            _soundSettings
                .Where(s => s.EventType == eventType)
                .ToList()
                .ForEach(s => PlaySound(s.SoundName, s.Volume));
        }

        private void PlaySound(string soundName, float volume)
        {
            try
            {
                Thrower.IfTrue(
                    string.IsNullOrEmpty(soundName), 
                    $"音イベントにサウンド名が指定されていません。");

                AudioManager.Instance.Play(soundName, gameObject).SetVolume(volume);
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"サウンド '{soundName}' " +
                    $"の再生中にエラーが発生しました: {ex.Message}");
            }
        }
    }
}