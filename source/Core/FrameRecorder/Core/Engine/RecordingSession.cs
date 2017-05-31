using System;
using System.Collections.Generic;

namespace UnityEngine.FrameRecorder
{
    public class RecordingSession : IDisposable
    {
        public Recorder m_Recorder;
        public GameObject m_RecorderGO;
        public List<UnityEngine.Object> m_ObjsOfInterest;
        public int m_FrameIndex; // count starts at 0.
        public double m_CurrentFrameStartTS;
        public double m_RecordingStartTS;

        public RecorderSettings settings { get { return m_Recorder.settings; } }
        public bool recording { get { return m_Recorder.recording; } }

        public bool BeginRecording()
        {
            if (!m_Recorder.BeginRecording(this))
                return false;
            m_Recorder.SignalSourcesOfStage(ERecordingSessionStage.BeginRecording, this);
            return true;
        }

        public virtual void EndRecording()
        {
            m_Recorder.SignalSourcesOfStage(ERecordingSessionStage.EndRecording, this);
            m_Recorder.EndRecording(this);
        }

        public void RecordFrame()
        {
            m_Recorder.SignalSourcesOfStage(ERecordingSessionStage.NewFrameReady, this);
            if (!m_Recorder.SkipFrame(this))
            {
                m_Recorder.RecordFrame(this);
                m_Recorder.recordedFramesCount++;
            }
            m_Recorder.SignalSourcesOfStage(ERecordingSessionStage.FrameDone, this);
        }

        public void PrepareNewFrame()
        {
            m_Recorder.SignalSourcesOfStage(ERecordingSessionStage.NewFrameStarting, this);
            m_Recorder.PrepareNewFrame(this);
        }

        public void Dispose()
        {
            if (m_Recorder != null)
            {
                if (recording)
                    EndRecording();

                UnityHelpers.Destroy(m_Recorder);
                UnityHelpers.Destroy(m_RecorderGO);
            }
        }
    }
}
