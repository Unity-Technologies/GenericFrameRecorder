using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Recorder.Input
{
    public abstract class ImageInputSettings : RecorderInputSetting
    {
        public EImageDimension maxSupportedSize { get; set; } // dynamic & contextual: do not save
        public EImageDimension m_OutputSize = EImageDimension.x720p_HD;
        public EImageAspect m_AspectRatio = EImageAspect.x16_9;
        public bool m_ForceEvenSize = false;

        public override bool isValid
        {
            get { return m_OutputSize <= maxSupportedSize; }
        }
    }
}