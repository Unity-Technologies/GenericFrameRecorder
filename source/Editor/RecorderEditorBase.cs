using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;

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

        protected override void OnInputGui( int inputIndex )
        {
            var inputs = (target as RecorderSettings).inputsSettings;
            var input = inputs[inputIndex];
            if (m_RTInputSelector.OnInputGui(ref input))
                ChangeInputSettings(inputIndex, input);                

            base.OnInputGui(inputIndex);
        }
    }
}
