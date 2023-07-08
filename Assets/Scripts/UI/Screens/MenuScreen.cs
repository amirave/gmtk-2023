using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Screens
{
    public class MenuScreen : MonoBehaviour
    {
        public Dictionary<string, string> textReplacements;
        public UnityEvent<string> OnButtonClick;

        private List<CustomText> _textObjects;
        
        private void Awake()
        {
            _textObjects = GetComponentsInChildren<CustomText>(true).ToList();
            
            var buttons = GetComponentsInChildren<CustomButton>(true);
            foreach (var button in buttons)
            {
                button.OnClick.AddListener(() => OnButtonClick.Invoke(button.GetButtonName()));
            }
        }

        public void SetTextReplacement(string query, string value)
        {
            textReplacements[query] = value;
            foreach (var text in _textObjects)
            {
                text.UpdateText();
            }
        }

        public string GetTextReplacement(string query)
        {
            return textReplacements[query];
        }
    }
}