using System;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Story
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private StoryFrame[] _storyFrames;
        [SerializeField] private Image _imageSlot;
        [SerializeField] private TypewriterText _textSlot;
        [SerializeField] private Image _fadeScreen;
        [SerializeField] private float _fadeDuration = 3f;

        private int _curFrameIndex = 0;
        private int _curTextIndex = 0;
        private float _curSpriteIndex = 0;

        private StoryFrame CurFrame => _storyFrames[_curFrameIndex];
        
        private void Awake()
        {
            NextText();
        }

        private void Update()
        {
            if (_curFrameIndex >= _storyFrames.Length)
                return;
            
            if (Input.GetMouseButtonDown(0))
                NextText();
            
            if (_curFrameIndex >= _storyFrames.Length)
                return;
            
            _curSpriteIndex += _storyFrames[_curFrameIndex].animSpeed * Time.deltaTime;
            var index = (int)_curSpriteIndex;

            if (_imageSlot != null)
                _imageSlot.sprite = CurFrame.animSprites[index % CurFrame.animSprites.Length];
        }

        private void NextText()
        {
            if (_textSlot.IsTyping())
            {
                _textSlot.SkipText();
            }
            else
            {
                _textSlot.TypeText(CurFrame.dialogLines[_curTextIndex]);
                _curTextIndex++;
                if (_curTextIndex >= CurFrame.dialogLines.Length)
                {
                    NextFrame().Forget();
                }
            }
        }

        private async UniTask NextFrame()
        {
            _curFrameIndex++;
            _curTextIndex = 0;
            _curSpriteIndex = 0;

            if (_curFrameIndex >= _storyFrames.Length)
            {
                var startTime = Time.time;
                var endTime = startTime + _fadeDuration;
                while (Time.time < endTime)
                {
                    _fadeScreen.color = Color.Lerp(new Color(0,0,0, 0), Color.black, 
                        Mathf.InverseLerp(startTime, endTime, Time.time));
                    
                    await UniTask.Yield();
                }

                await UniTask.Delay(1000);
                
                LifetimeManager.Instance.LoadScene(SceneIndexes.GAME).Forget();
            }
        }
    }
}