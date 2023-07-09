using System;
using System.Text.RegularExpressions;
using TMPro;
using UI.Screens;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class CustomText : MonoBehaviour
    {
        private static readonly string MATCH_PATTERN = @"\${(\w+)}";
        
        private string _originalText;
        private TMP_Text _tmpText;

        private Func<string, string> _query;
        private MenuScreen _menuScreen;

        public void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            _originalText = _tmpText.text;

            _menuScreen = GetComponentInParent<MenuScreen>();
        }

        public void UpdateText()
        {
            string Evaluator(Match match)
            {
                var variableName = match.Groups[1].Value; // Extract the variable name from the matched pattern
                return _menuScreen.GetTextReplacement(variableName); // Query the value of the variable
            }

            _tmpText.text = Regex.Replace(_originalText, MATCH_PATTERN, Evaluator);
        }
    }
}