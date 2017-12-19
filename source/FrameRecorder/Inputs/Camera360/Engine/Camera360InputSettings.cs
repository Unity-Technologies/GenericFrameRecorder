#if UNITY_2018_1_OR_NEWER

using System;

namespace UnityEngine.Recorder.Input
{

    public class Camera360InputSettings : ImageInputSettings
    {
        public EImageSource source = EImageSource.MainCamera;
        public string m_CameraTag;
        public bool m_FlipFinalOutput = false;
        public bool m_RenderStereo = true;
        public float m_StereoSeparation = 0.065f;
        public int m_MapSize = 256;
        public int m_OutputSizePower = 1024;

        public override Type inputType
        {
            get { return typeof(Camera360Input); }
        }

        public override bool isValid
        {
            get
            {
                return base.isValid &&
                    (source != EImageSource.TaggedCamera || !string.IsNullOrEmpty(m_CameraTag))
                    && m_OutputSizePower == (1 << (int)Math.Log(m_OutputSizePower, 2))
                    && m_OutputSizePower > 0
                    && m_OutputSizePower <= 8 * 1024
                    && m_MapSize > 0
                    && m_MapSize == (1 << (int)Math.Log(m_MapSize, 2))
                    && m_MapSize < 8 * 1024;
            }
        }
    }

}

#endif