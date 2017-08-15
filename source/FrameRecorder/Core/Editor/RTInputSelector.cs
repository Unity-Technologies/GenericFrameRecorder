using System;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UnityEditor.FrameRecorder
{
    public class RTInputSelector
    {
        string title { get; set; }
        bool enforceEvenSize { get; set; }
        string[] candidates = { "Camera output", "Offscreen rendering", "Render Texture" };

        public RTInputSelector(string title, bool enforceEvenSize )
        {
            this.title = title;
            this.enforceEvenSize = enforceEvenSize;
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

                if (newSettings is CBRenderTextureInputSettings)
                    (newSettings as CBRenderTextureInputSettings).m_PadSize = enforceEvenSize;
                if (newSettings is AdamBeautyInputSettings)
                    (newSettings as AdamBeautyInputSettings).m_ForceEvenSize = enforceEvenSize;

                return true;
            }

            return false;
        }
    }

}