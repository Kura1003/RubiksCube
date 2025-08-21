using TMPro;
using UnityEngine;

namespace Taki.Utility
{
    public class RandomTextApplier : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textMeshPro;

        [SerializeField]
        private int _length = 10;

        [SerializeField]
        private CharType _charType = CharType.UpperCase;

        private void Awake()
        {
            ApplyRandomText();
        }

        public void ApplyRandomText()
        {
            string newText = RandomTextGenerator.Generate(_length, _charType);
            _textMeshPro.text = newText;
            _textMeshPro.color = RandomUtility.GetColor();
        }
    }
}