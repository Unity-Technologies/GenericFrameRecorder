using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UTJ.FrameCapturer.Recorders
{
    public class RecorderEditorBase: RecorderEditor
    {
        public string m_BaseFileName;
        public string m_DestinationPath;

        protected RTInputSelector m_RTInputSelector;

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

            m_RTInputSelector = new RTInputSelector(target as RecorderSettings, "Pixels");
        }

        /* can't use this at the moment as the FC flips the image horizontally, but the offscreen input does not offer that option.
        protected override void OnInputGui( int inputIndex )
        {
            
            var input = m_Inputs.GetArrayElementAtIndex(inputIndex).objectReferenceValue as RecorderInputSetting;
            if (m_RTInputSelector.OnInputGui(ref input))
            {
                if( input is CBRenderTextureInputSettings )
                    (input as CBRenderTextureInputSettings).m_FlipVertical = false;

                ChangeInputSettings(inputIndex, input);                
            }


            base.OnInputGui(inputIndex);
        }
        */
        protected override EFieldDisplayState GetFieldDisplayState( SerializedProperty property)
        {
            if( property.name == "m_FlipVertical" )
                return EFieldDisplayState.Hidden;

            return base.GetFieldDisplayState(property);
        }

    }
}
