using System;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UnityEditor.FrameRecorder
{
    public class RTInputSelector
    {
        string title { get; set; }
        string[] candidates = { "Camera output", "Offscreen rendering", "Render Texture" };
        RecorderSettings recSettings;

        public RTInputSelector( RecorderSettings recSettings, string title )
        {
            this.recSettings = recSettings;
            this.title = title;
        }

        public bool OnInputGui( ref RecorderInputSetting input)
        {
            var index = input.GetType() == typeof(CBRenderTextureInputSettings) ? 0 :
                input.GetType() == typeof(AdamBeautyInputSettings) ? 1 : 2;
            var newIndex = EditorGUILayout.Popup("Collection method", index, candidates);

            if (index != newIndex)
            {
                switch (newIndex)
                {
                    case 0:
                        input = recSettings.NewInputSettingsObj<CBRenderTextureInputSettings>( title );
                        break;
                    case 1:
                        input = recSettings.NewInputSettingsObj<AdamBeautyInputSettings>( title );
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