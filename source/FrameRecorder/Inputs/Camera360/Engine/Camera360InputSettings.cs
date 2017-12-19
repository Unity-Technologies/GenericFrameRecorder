#if UNITY_2018_1_OR_NEWER

using System;
using System.Collections.Generic;

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

        public override bool ValidityCheck( List<string> errors )
        {
            bool ok = base.ValidityCheck(errors);

            if (source == EImageSource.TaggedCamera && string.IsNullOrEmpty(m_CameraTag))
            {
                ok = false;
                errors.Add("Missing camera tag");
            }

            if (m_OutputSizePower != (1 << (int)Math.Log(m_OutputSizePower, 2)))
            {
                ok =false;
                errors.Add("Output size must be a power of 2.");
            }

            if (m_OutputSizePower < 128 || m_OutputSizePower > 8 * 1024)
            {
                ok = false;
                errors.Add( string.Format( "Output size must fall between {0} and {1}.", 128, 8*1024 ));
            }

            if (m_MapSize != (1 << (int)Math.Log(m_MapSize, 2)))
            {
                ok = false;
                errors.Add("Cube Map size must be a power of 2.");
            }

            if( m_MapSize < 16 || m_MapSize > 8 * 1024 )
            {
                ok = false;
                errors.Add( string.Format( "Cube Map size must fall between {0} and {1}.", 16, 8*1024 ));
            }

            if (m_RenderStereo && m_StereoSeparation < float.Epsilon)
            {
                ok = false;
                errors.Add("Stereo separation value is too small.");
            }

            return ok;
        }
    }

}

#endif