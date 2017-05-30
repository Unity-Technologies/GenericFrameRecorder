using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

public class CBRenderTextureInputSettings : InputSettings<CBRenderTextureInput>
{
    public EImageSource source = EImageSource.GameDisplay;
    public EImageDimension m_RenderSize = EImageDimension.x720p_HD;
    public string m_CameraTag;
    public RenderTexture m_SourceRTxtr;
}
