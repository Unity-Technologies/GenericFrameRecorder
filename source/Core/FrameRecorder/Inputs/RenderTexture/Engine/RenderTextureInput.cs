using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;
using UnityEngine.Rendering;

namespace UnityEngine.FrameRecorder.Input
{
    public class RenderTextureInput : BaseRenderTextureInput
    {
        RenderTextureInputSettings cbSettings
        {
            get { return (RenderTextureInputSettings)settings; }
        }

        public override void BeginRecording(RecordingSession session)
        {
            if (cbSettings.m_SourceRTxtr == null)
                throw new Exception("No Render Texture object provided as source");

            outputHeight = cbSettings.m_SourceRTxtr.height;
            outputWidth = cbSettings.m_SourceRTxtr.width;
            outputRT = cbSettings.m_SourceRTxtr;
        }
    }
}