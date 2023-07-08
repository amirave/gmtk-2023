using UnityEngine;
using UnityEngine.UI;

namespace UI.Screens
{
    public class HudMenuScreen : MenuScreen
    {
        [SerializeField] private Image _currentEmotion;

        public void SetEmotion(Sprite sprite)
        {
            _currentEmotion.sprite = sprite;
        }
    }
}