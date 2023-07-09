using System.Collections;
using System.Collections.Generic;
using Audio;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    void Start()
    {
        _startButton.onClick.AddListener(StartGame);
        AudioManager.Instance.PlayMusicTrack("menu_music");
    }

    private void StartGame()
    {
        LifetimeManager.Instance.LoadScene(SceneIndexes.STORY).Forget();
    }

    void Update()
    {
        AudioManager.Instance.ToggleChannelInternal(AudioChannel.Music, _musicSlider.value, false);
        AudioManager.Instance.ToggleChannelInternal(AudioChannel.Fx, _sfxSlider.value, false);
    }
}
