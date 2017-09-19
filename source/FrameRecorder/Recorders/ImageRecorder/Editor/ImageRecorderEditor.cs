using System;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(ImageRecorderSettings))]
    public class ImageRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;
        RTInputSelector m_RTInputSelector;
        
        [MenuItem("Tools/Recorder/Video")]
        static void ShowRecorderWindow()
        {
            RecorderWindow.ShowAndPreselectCategory("Video");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
                return;

            m_RTInputSelector = new RTInputSelector( target as RecorderSettings, "Pixels");

            var pf = new PropertyFinder<ImageRecorderSettings>(serializedObject);
            m_OutputFormat = pf.Find(w => w.m_OutputFormat);
        }

        protected override void OnEncodingGroupGui()
        {
            // hiding this group by not calling parent class's implementation.  
        }

        protected override void OnInputGui( int inputIndex)
        {
            var inputs = (target as RecorderSettings).inputsSettings;
            var input = inputs[inputIndex];
            if (m_RTInputSelector.OnInputGui(ref input))
                ChangeInputSettings(inputIndex, input);                

            base.OnInputGui(inputIndex);
        }

        protected override void OnOutputGui()
        {
            AddProperty(m_OutputFormat, () => EditorGUILayout.PropertyField(m_OutputFormat, new GUIContent("Output format")));
            base.OnOutputGui();
        }
    }
}
