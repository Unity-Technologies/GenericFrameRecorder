using System;

namespace UnityEngine.FrameRecorder
{
    public abstract class  RecorderInputSetting : ScriptableObject
    {
        public abstract Type sourceType { get; }
    }

    public class InputSettings<TSource> : RecorderInputSetting
    {
        public override Type sourceType
        {
            get { return typeof(TSource); }
        }
    }
}
