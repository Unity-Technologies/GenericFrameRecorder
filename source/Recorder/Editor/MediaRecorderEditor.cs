using System;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;

namespace UnityEditor.Recorder
{
    [CustomEditor(typeof(MediaRecorderSettings))]
    public class MediaRecorderEditor : RecorderEditor
    {
        SerializedProperty m_OutputFormat;
        SerializedProperty m_FlipVertical;
        RTInputSelector m_RTInputSelector;

        [MenuItem("Window/Recorder/Video")]
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

            var pf = new PropertyFinder<MediaRecorderSettings>(serializedObject);
            m_OutputFormat = pf.Find(w => w.m_OutputFormat);
        }

        protected override void OnEncodingGroupGui()
        {
            // hiding this group by not calling parent class's implementation.  
        }

        protected override void OnInputGui(int inputIndex)
        {
            if (inputIndex == 0)
            {
                var input = (target as RecorderSettings).inputsSettings[inputIndex];
                if (m_RTInputSelector.OnInputGui(ref input))
                    ChangeInputSettings(inputIndex, input);                
            }

            base.OnInputGui(inputIndex);
        }

        protected override void OnOutputGui()
        {
            AddProperty(m_OutputFormat, () => EditorGUILayout.PropertyField(m_OutputFormat, new GUIContent("Output format")));

            base.OnOutputGui();
        }

        protected override EFieldDisplayState GetFieldDisplayState(SerializedProperty property)
        {
            if (property.name == "m_FlipVertical" || property.name == "m_CaptureEveryNthFrame" )
                return EFieldDisplayState.Hidden;
            if (property.name == "m_FrameRateMode" )
                return EFieldDisplayState.Disabled;

            if (property.name == "m_AllowTransparency")
            {
                return (target as MediaRecorderSettings).m_OutputFormat == MediaRecorderOutputFormat.MP4 ? EFieldDisplayState.Disabled : EFieldDisplayState.Enabled;
            }


            return EFieldDisplayState.Enabled;
        }
    }
}
