using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Screens
{
    public class MenuScreen : MonoBehaviour
    {
        public Dictionary<string, string> textReplacements;
        public UnityEvent<string> OnButtonClick;

        private List<CustomText> _textObjects;
        private Dictionary<string, List<Action>> _callbacks;

        protected virtual void Awake()
        {
            _textObjects = GetComponentsInChildren<CustomText>(true).ToList();
            textReplacements = new();
            _callbacks = new();

            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                button.onClick.AddListener( () => ButtonClick(button.name));
            }
        }

        private void ButtonClick(string name)
        {
            OnButtonClick.Invoke(name);
            if (_callbacks.ContainsKey(name))
                _callbacks[name].ForEach(callback => callback.Invoke());
        }

        public void SubscribeToButton(string name, Action callback)
        {
            if (_callbacks.ContainsKey(name) == false)
                _callbacks.Add(name, new List<Action>());
            
            _callbacks[name].Add(callback);
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