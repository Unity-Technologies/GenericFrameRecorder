using System;

namespace UnityEngine.FrameRecorder
{
    public class InputBinder : RecorderInputSetting
    {
        public string m_TypeName;
        public override Type inputType
        {
            get { return Type.GetType(m_TypeName); }
        }

        public override bool isValid
        {
            get { return false; }
        }
        
    }
}
