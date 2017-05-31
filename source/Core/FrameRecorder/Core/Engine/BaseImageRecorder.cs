using System.Collections.Generic;
using UnityEngine.FrameRecorder.Input;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace UnityEngine.FrameRecorder
{
    public abstract class BaseImageRecorder<TSettings> : Recorder where TSettings : RecorderSettings
    {
        [SerializeField]
        protected TSettings m_Settings;
        public override RecorderSettings settings
        {
            get { return m_Settings; }
            set { m_Settings = (TSettings)value; }
        }

        public override List<RecorderInputSetting> DefaultInputs()
        {
            var settings = new List<RecorderInputSetting>();
            var setting = ScriptableObject.CreateInstance(typeof(AdamBeautyInputSettings)) as AdamBeautyInputSettings;

            settings.Add(setting);
            return settings;
        }

    }
}
