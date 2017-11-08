using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Recorder.Input;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Media;
#endif

namespace UnityEngine.Recorder
{

    public enum MediaRecorderOutputFormat
    {
        MP4,
        WEBM
    }

    [ExecuteInEditMode]
    public class MediaRecorderSettings : RecorderSettings
    {
        public MediaRecorderOutputFormat m_OutputFormat = MediaRecorderOutputFormat.MP4;
#if UNITY_2018_1_OR_NEWER
        public UnityEditor.Media.VideoBitRateMode m_VideoBitRateMode = UnityEditor.Media.VideoBitRateMode.High;
#endif
        public bool m_AppendSuffix = false;

        MediaRecorderSettings()
        {
            m_BaseFileName.pattern = "movie.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultInputSettings()
        {
            return new List<RecorderInputSetting>()
            {
                NewInputSettingsObj<CBRenderTextureInputSettings>("Pixels"),
                NewInputSettingsObj<AudioInputSettings>("Audio")
            };
        }

        public override bool isValid
        {
            get { return base.isValid && !string.IsNullOrEmpty(m_DestinationPath.GetFullPath()); }
        }

        public override RecorderInputSetting NewInputSettingsObj(Type type, string title)
        {
            var obj = base.NewInputSettingsObj(type, title);
            if (type == typeof(CBRenderTextureInputSettings))
            {
                (obj as CBRenderTextureInputSettings).m_ForceEvenSize = true;
                (obj as CBRenderTextureInputSettings).m_FlipFinalOutput = Application.platform == RuntimePlatform.OSXEditor;
            }
            if (type == typeof(RenderTextureSamplerSettings))
            {
                (obj as RenderTextureSamplerSettings).m_ForceEvenSize = true;
            }

            return obj ;
        }

    }
}
