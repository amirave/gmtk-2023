using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class LifetimeManager : MonoBehaviour
    {
        public static LifetimeManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public async UniTask LoadScene(int sceneIndex)
        {
            // Wait until loading screen fully covers the view
            if (Random.value > 0.5f)
                await LoadingManager.Instance.Require();
            else
                await LoadingManager.Instance.Require(Input.mousePosition);
        
            var operation = SceneManager.LoadSceneAsync(sceneIndex);
            await UniTask.WaitUntil(() => operation.isDone);
        
            LoadingManager.Instance.Release().Forget();
        }
    }
}