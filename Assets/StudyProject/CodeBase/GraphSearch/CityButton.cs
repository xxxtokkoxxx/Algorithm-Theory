using System;
using TMPro;
using UnityEngine;

namespace StudyProject.CodeBase
{
    public class CityButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private Action<string> _onClick;

        public void Initialize(string text, Action<string> onClick)
        {
            _onClick = onClick;

            if (_text != null)
                _text.text = text;
        }

        public void OnClick()
        {
            if (_text != null)
                _onClick?.Invoke(_text.text);
        }
    }
}