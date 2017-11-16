using System;

namespace UnityEngine.Recorder.Input
{
    public class RenderTextureInputSettings : ImageInputSettings
    {
        public RenderTexture m_SourceRTxtr;

        public override Type inputType
        {
            get { return typeof(RenderTextureInput); }
        }

        public override bool isValid
        {
            get
            {
                return m_SourceRTxtr != null; 
            }
        }

    }
}
