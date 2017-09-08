using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;
using UnityEngine.SceneManagement;
using Component = System.ComponentModel.Component;

namespace UnityEditor.FrameRecorder
{
    [Serializable]
    [CustomEditor(typeof(AnimationRecorderSettings))]
    public class AnimationRecorderEditor: RecorderEditor
    {
        [MenuItem("Tools/Recorder/Animation")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Animation");
        }

        
        protected override void OnEncodingGroupGui()
        {}


        protected override void OnInputGui()
        {
            var inputs = (target as RecorderSettings).inputsSettings;

            for (int i = 0; i < inputs.Count; i++)
            {
                OnInputGui(i);
            }

            var aRecorderSettings = target as AnimationRecorderSettings;


            EditorGUI.BeginChangeCheck();
            GameObject newgo = EditorGUILayout.ObjectField(null, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck() && newgo !=null)
            {
                var newSettings = aRecorderSettings.NewInputSettingsObj<AnimationInputSettings>(newgo.name);
                newSettings.gameObject = newgo;
                newSettings.enabled = true;
                aRecorderSettings.inputsSettings.Add(newSettings);
            }
        }
        
    }
}