using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI
{
    public class LoadingManager : MonoBehaviour
    {
        private enum State
        {
            Starting,
            Started,
            Ending,
            Ended
        }
    
        private readonly int FADE_IN_ANIMATION_HASH = Animator.StringToHash("FadeIn");
        private readonly int FADE_OUT_ANIMATION_HASH = Animator.StringToHash("FadeOut");
    
        public static LoadingManager Instance { get; private set; }

        private bool _isShowing = false;
        private Action<int> _currentProgress;
        private State _state = State.Ended;
        private GameObject _currentLoadScreen;

        // TODO a separate interface for different loading screens (percentage, slider bar, etc). i think its enough for now tho
        [SerializeField] private GameObject _spikeLoadScreen;
        [SerializeField] private GameObject _circleLoadScreen;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public async UniTask Require()
        {
            await Display(_spikeLoadScreen, _spikeLoadScreen.GetComponent<Animator>());
        }

        public async UniTask Require(Vector3 screenPosition)
        {
            _circleLoadScreen.transform.GetChild(0).position = screenPosition;
            await Display(_circleLoadScreen, _circleLoadScreen.GetComponent<Animator>());
        }

        private async UniTask Display(GameObject screen, Animator animator)
        {
            _currentLoadScreen = screen;
            
            if (_state is State.Started or State.Starting)
                return;

            if (_state == State.Ending)
                await UniTask.WaitUntil(() => _state == State.Ended);
        
            _isShowing = true;
            screen.SetActive(true);
        
            animator.Play(FADE_IN_ANIMATION_HASH);
            _state = State.Starting;

            await UniTask.Delay((int)(1000 * animator.GetCurrentAnimatorStateInfo(0).length));
        
            _state = State.Started;
        }

        public async UniTask Release()
        {
            if (_state is State.Ended or State.Ending)
                return;
        
            if (_state == State.Starting)
                await UniTask.WaitUntil(() => _state == State.Started);
        
            _currentLoadScreen.GetComponent<Animator>().Play(FADE_OUT_ANIMATION_HASH);
            _state = State.Ending;

            await UniTask.Delay((int)(1000 * _currentLoadScreen.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length));
        
            _isShowing = false;
            _currentLoadScreen.SetActive(false);
            _state = State.Ended;
        }

        public bool IsShowing() => _isShowing;
    }
}
