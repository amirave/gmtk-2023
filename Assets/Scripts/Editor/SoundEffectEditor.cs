using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(SoundEffect))]
public class SoundEffectEditor : Editor
{
    [SerializeField] private AudioSource _previewer;

    public void OnEnable()
    {
        _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio Preview", HideFlags.HideAndDontSave, typeof(AudioSource))
            .GetComponent<AudioSource>();
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        var targetSfx = (SoundEffect)target;
        
        MinMaxSliderWithNumbers(ref targetSfx.minVolume, ref targetSfx.maxVolume, 0 ,2);
        MinMaxSliderWithNumbers(ref targetSfx.minPitch, ref targetSfx.maxPitch, 0 ,2);

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        if (GUILayout.Button("Preview"))
            targetSfx.Play(_previewer);
        EditorGUI.EndDisabledGroup();
    }

    private void MinMaxSliderWithNumbers(ref float minValue, ref float maxValue, float minLimit, float maxLimit)
    {
        EditorGUILayout.BeginHorizontal();
        {
            minValue = Utils.RoundToDecimal(EditorGUILayout.FloatField(minValue, new []{GUILayout.Width(50)}), 2);
            EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, minLimit, maxLimit);
            maxValue = Utils.RoundToDecimal(EditorGUILayout.FloatField(maxValue, new []{GUILayout.Width(50)}), 2);
        }
        EditorGUILayout.EndHorizontal();
    }
}
