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
        public MediaRecorderOutputFormat m_OutputFormat = MediaRecorderOutputFormat.MP4;
        public bool m_AppendSuffix = false;

        MediaRecorderSettings()
        {
            m_BaseFileName.pattern = "movie_<0000>.<ext>";
        }

        public override List<RecorderInputSetting> GetDefaultSourcesSettings()
        {
            var defaultTextureSettings = ScriptableObject.CreateInstance<CBRenderTextureInputSettings>();
            defaultTextureSettings.m_FlipVertical = true;
            var defaultAudioSettings = ScriptableObject.CreateInstance<AudioInputSettings>();
            return new List<RecorderInputSetting>() { defaultTextureSettings, defaultAudioSettings };
        }

        public override bool isValid
        {
            get { return base.isValid && !string.IsNullOrEmpty(m_DestinationPath.GetFullPath()); }
        }

    }
}
