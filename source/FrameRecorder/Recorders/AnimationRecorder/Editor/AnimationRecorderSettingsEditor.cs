using System;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;

namespace UnityEditor.FrameRecorder
{
    [Serializable]
    [CustomEditor(typeof(AnimationRecorderSettings))]
    public class AnimationRecorderSettingsEditor: RecorderEditor
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
            var aRecorderSettings = target as AnimationRecorderSettings;
            var inputs = aRecorderSettings.inputsSettings;

            for (int i = 0; i < inputs.Count; i++)
            {              
                OnInputGui(i);
                if (GUILayout.Button("Remove",GUILayout.MaxWidth(100)))
                {
                    aRecorderSettings.inputsSettings.Remove(inputs[i]);
                }
            }

            if (GUILayout.Button("Add", GUILayout.MaxWidth(100)))
            {
                var newSettings = aRecorderSettings.NewInputSettingsObj<AnimationInputSettings>("Animation");
                aRecorderSettings.inputsSettings.Add(newSettings);
            }           
        }

        protected override void OnOutputGui()
        {
            var aRecorderSettings = target as AnimationRecorderSettings;
            aRecorderSettings.outputPath = EditorGUILayout.TextField("Output Path", aRecorderSettings.outputPath);
            aRecorderSettings.take = EditorGUILayout.IntField("Take", aRecorderSettings.take);
        }
        
    }
}