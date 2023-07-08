using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
                LoadGame().Forget();
        }

        // private void PushGameState<TState, TContext, TConfig>(TContext context)
        //     where TContext : class
        //     where TConfig : class
        //     where TState : GameStateBase<TContext, TConfig>, new()
        // {
        //     var newState = new TState();
        //     
        //     if (newState is GameStateBase<TContext, TConfig> newStateBase)
        //     {
        //         _gameStates.Peek().Suspend();
        //         _gameStates.Push(newStateBase);
        //     }
        // }

        public async UniTask LoadGame()
        {
            // TODO If i'll ever add a loading bar uncomment this
            // Wait until loading screen fully covers the view
            if (Random.value > 0.5f)
                await LoadingManager.Instance.Require();
            else
                await LoadingManager.Instance.Require(Input.mousePosition);
        
            var operation = SceneManager.LoadSceneAsync(Random.value > 0.5f ? SceneIndexes.GAME : SceneIndexes.TITLE_SCREEN);
            await UniTask.WaitUntil(() => operation.isDone);
        
            LoadingManager.Instance.Release().Forget();
        }
        
        // public float GetSceneLoadProgress()
        // {
        //     return _scenesLoading.Sum(operation => operation.progress);
        // }
    }
}