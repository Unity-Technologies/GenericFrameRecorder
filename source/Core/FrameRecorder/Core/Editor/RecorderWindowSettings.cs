using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;

/// <summary>
/// This is just a helper class that should disappear once we have a proper way of saving the recorder window settings...
/// </summary>
public class RecorderWindowSettings : ScriptableObject
{
    public FrameRecorderSettings m_Settings;
}
