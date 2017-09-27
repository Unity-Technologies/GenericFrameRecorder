using System;
using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;

namespace UnityEditor.Recorder
{
    public class RTInputSelector
    {
        string title { get; set; }
        string[] candidates = { "Frame buffer", "Sampling", "Render Texture" };
        RecorderSettings recSettings;

        public RTInputSelector( RecorderSettings recSettings, string title )
        {
            this.recSettings = recSettings;
            this.title = title;
        }

        public bool OnInputGui( ref RecorderInputSetting input)
        {
            var index = input.GetType() == typeof(CBRenderTextureInputSettings) ? 0 :
                input.GetType() == typeof(RenderTextureSamplerSettings) ? 1 : 2;
            var newIndex = EditorGUILayout.Popup("Collection method", index, candidates);

            if (index != newIndex)
            {
                switch (newIndex)
                {
                    case 0:
                        input = recSettings.NewInputSettingsObj<CBRenderTextureInputSettings>( title );
                        break;
                    case 1:
                        input = recSettings.NewInputSettingsObj<RenderTextureSamplerSettings>( title );
                        break;
                    case 2:
                        input = recSettings.NewInputSettingsObj<RenderTextureInputSettings>( title );
                        break;
                }
                return true;
            }

            return false;
        }
    }

}