using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Story
{
    public class TypewriterText : MonoBehaviour
    {
        [SerializeField] private float _charDelay = 0.125f;
        
        private TMP_Text _uiText;
        private string _currentText;
        private int _textProgress;
        private bool _isTyping;

        private CancellationTokenSource _cts;

        public void TypeText(string newText)
        {
            _uiText = GetComponent<TMP_Text>();
            _cts?.Cancel();

            _currentText = newText;
            _cts = new CancellationTokenSource();
            PlayText(newText, _cts.Token).Forget();
        }

        public void SkipText()
        {
            _cts.Cancel();
        }

        private async UniTask PlayText(string newText, CancellationToken token)
        {
            _isTyping = true;
            _uiText.text = "";
            
            // foreach (var c in newText) 
            // {
            //     _uiText.text += c;
            //     await UniTask.Delay(1000 * (int)_charDelay, cancellationToken: token);
            //     if (token.IsCancellationRequested)
            //     {
            //         _isTyping = false;
            //         _uiText.text = newText;
            //         return;
            //     }
            // }
            _uiText.text = newText;

            _isTyping = false;
            _uiText.text = newText;
        }

        public bool IsTyping()
        {
            return _isTyping;
        }
    }
}