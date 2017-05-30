using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Recorder.FrameRecorder;
using UnityEngine.Recorder.FrameRecorder.DataSource;

namespace UTJ.FrameCapturer.Recorders
{
    public abstract class BaseFCRecorderSettings : FrameRecorderSettings
    {
        public string m_BaseFileName = "file";
        public string m_DestinationPath = "Recorder";

        public override bool isValid
        {
            get
            {
                return base.isValid && !string.IsNullOrEmpty(m_DestinationPath) && !string.IsNullOrEmpty(m_BaseFileName);
            }
        }
    }
}
