using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmotionalSlider : MonoBehaviour
{
    [SerializeField] private Sprite[] _emotionSprites;

    private Slider _slider;
    private Image _handle;
    
    void Start()
    {
        _slider = GetComponent<Slider>();
        _handle = _slider.handleRect.GetComponent<Image>();
    }

    void Update()
    {
        _handle.sprite = _emotionSprites[^Mathf.CeilToInt(_slider.value * _emotionSprites.Length)];
    }
}
