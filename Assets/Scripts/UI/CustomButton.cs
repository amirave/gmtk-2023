using System;
using Audio;
using LlamAcademy.Spring;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace UI
{
    public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public UnityEvent OnClick;
        [SerializeField] private SpringAnimParams _hoverAnimParams;
        [SerializeField] private SpringAnimParams _clickAnimParams;

        private FloatSpring _spring;
        private float _springValue;

        [SerializeField]
        private string _buttonName = string.Empty;

        private Vector3 _scaleModifier;
        private Vector3 _rotationModifier;

        public void Start()
        {
            _spring = new FloatSpring
            {
                Damping = 5,
                Stiffness = 100,
                StartValue = 0,
                EndValue = 0
            };
        }

        void Update()
        {
            // Debug.Log(_spring.CurrentVelocity);
            _springValue = _spring.Evaluate(Time.deltaTime);

            // transform.localScale = Utils.ApplyFunction(_springValue * _scaleModifier, x => Mathf.Pow(2.7f, x));
            transform.localScale = Vector3.one + _springValue * _scaleModifier;
            transform.rotation = Quaternion.Euler(_springValue * _rotationModifier);

            if (Input.GetKeyDown(KeyCode.T))
            {
                _spring.UpdateEndValue(_spring.EndValue, 5);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (AudioManager.Instance)
                AudioManager.Instance.PlayEffect(AudioNames.UI_CLICK_ID);
        
            ApplyAnimParams(_hoverAnimParams);
            _spring.UpdateEndValue(1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ApplyAnimParams(_hoverAnimParams);
            _spring.UpdateEndValue(0);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (OnClick != null)
                OnClick.Invoke();
            
            if (AudioManager.Instance)
                AudioManager.Instance.PlayEffect(AudioNames.UI_CLICK_ID);

            ApplyAnimParams(_clickAnimParams);
            _spring.UpdateEndValue(0);
        }

        public string GetButtonName()
        {
            if (string.IsNullOrEmpty(_buttonName))
                return gameObject.name;

            return _buttonName;
        }

        private void ApplyAnimParams(SpringAnimParams animParams)
        {
            _spring.Damping = animParams.damping;
            _spring.Stiffness = animParams.stiffness;
            _spring.UpdateEndValue(_spring.EndValue, animParams.initialVelocity);
            _spring.CurrentVelocity = animParams.initialVelocity;

            _rotationModifier = animParams.rotationModifier;
            _scaleModifier = animParams.scaleModifier;
        }
    }
}
