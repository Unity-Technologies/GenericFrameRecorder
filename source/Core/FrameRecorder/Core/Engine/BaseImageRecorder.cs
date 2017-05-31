using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UnityEngine.Recorder.FrameRecorder
{
    public abstract class BaseImageRecorder<TSettings> : Recorder where TSettings : FrameRecorderSettings
    {
        [SerializeField]
        protected TSettings m_Settings;
        public override FrameRecorderSettings settings
        {
            get { return m_Settings; }
            set { m_Settings = (TSettings)value; }
        }

        public override List<RecorderInputSetting> DefaultSourceSettings()
        {
            var settings = new List<RecorderInputSetting>();
            var setting = ScriptableObject.CreateInstance(typeof(AdamBeautyInputSettings)) as AdamBeautyInputSettings;

            settings.Add(setting);
            return settings;
        }

    }
}
