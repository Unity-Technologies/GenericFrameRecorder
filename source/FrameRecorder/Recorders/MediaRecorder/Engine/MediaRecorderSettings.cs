using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.FrameRecorder.Input;

namespace UnityEngine.FrameRecorder
{

    public enum MediaRecorderOutputFormat
    {
        MP4,
        WEBM
    }

    [ExecuteInEditMode]
    public class MediaRecorderSettings : RecorderSettings
    {
        public string                       m_BaseFileName    = "recorded_movie";
        public string                       m_DestinationPath = "Assets/Recorded";
        public MediaRecorderOutputFormat    m_OutputFormat    = MediaRecorderOutputFormat.MP4;
	public bool                         m_AppendSuffix    = false;

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            var defaultTextureSettings = ScriptableObject.CreateInstance<CBRenderTextureInputSettings>();
            defaultTextureSettings.m_FlipVertical = true;
            var defaultAudioSettings = ScriptableObject.CreateInstance<AudioInputSettings>();
            return new List<RecorderInputSetting>() { defaultTextureSettings, defaultAudioSettings };
        }

        public override bool isValid
        {
            get
            {
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath) && !string.IsNullOrEmpty(m_BaseFileName);
            }
        }

    }
}
