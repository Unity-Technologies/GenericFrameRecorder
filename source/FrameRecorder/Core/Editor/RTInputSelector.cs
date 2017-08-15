using System;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UnityEditor.FrameRecorder
{
    public class RTInputSelector
    {
        public string title { get; set; }
        string[] candidates = { "Camera output", "Offscreen rendering", "Render Texture" };

        public RTInputSelector(string title)
        {
            this.title = title;
        }

        public bool OnInputGui( ref RecorderInputSetting input)
        {
            var index = input.GetType() == typeof(CBRenderTextureInputSettings) ? 0 :
                input.GetType() == typeof(AdamBeautyInputSettings) ? 1 : 2;
            var newIndex = EditorGUILayout.Popup("Collection method", index, candidates);

            if (index != newIndex)
            {
                Type newType = null;
                switch (newIndex)
                {
                    case 0:
                        newType = typeof(CBRenderTextureInputSettings);
                        break;
                    case 1:
                        newType = typeof(AdamBeautyInputSettings);
                        break;
                    case 2:
                        newType = typeof(RenderTextureInputSettings);
                        break;
                }
                var newSettings = ScriptableObject.CreateInstance(newType) as RecorderInputSetting;
                newSettings.m_DisplayName = title;
                input = newSettings;
                return true;
            }

            return false;
        }
    }

}