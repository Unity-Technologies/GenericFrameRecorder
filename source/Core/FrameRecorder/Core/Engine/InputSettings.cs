using System;

namespace UnityEngine.FrameRecorder
{
    public abstract class  RecorderInputSetting : ScriptableObject
    {
        public abstract Type inputType { get; }
    }

    public class InputSettings<TInput> : RecorderInputSetting
    {
        public override Type inputType
        {
            get { return typeof(TInput); }
        }
    }
}
