using System;
using System.Collections.Generic;

namespace UnityEngine.FrameRecorder
{
    public enum ERecordingSessionStage
    {
        BeginRecording,
        NewFrameStarting,
        NewFrameReady,
        FrameDone,
        EndRecording,
    }

    public abstract class Recorder : ScriptableObject
    {
        double m_OriginalCaptureFrameRate;

        public abstract List<RecorderInputSetting> DefaultInputs();

        public int recordedFramesCount { get; set; }
        
        protected List<RecorderInput> m_Sources;

        public virtual void Reset()
        {
            recordedFramesCount = 0;
            recording = false;
        }

        protected virtual void OnDestroy()
        {
        }

        public abstract RecorderSettings settings { get; set; }

        // returns true if recording is starting. false if failed to begin recording or was already recording
        public virtual bool BeginRecording(RecordingSession session)
        {
            if (recording)
                return false;

            if (settings.m_Verbose)
                Debug.Log(string.Format("Recorder {0} starting to record", GetType().Name));

            m_OriginalCaptureFrameRate = Time.captureFramerate;
            var fixedRate = settings.m_FrameRateMode == FrameRateMode.Constant ? (int)settings.m_FrameRate : m_OriginalCaptureFrameRate;
            if (fixedRate != m_OriginalCaptureFrameRate)
            {
                if (Time.captureFramerate > 0)
                    Debug.LogWarning(string.Format("Frame Recorder {0} is set to record at a fixed rate and another component has already set a conflicting value for [Time.captureFramerate], new value being applied : {1}!", GetType().Name, fixedRate));
                Time.captureFramerate = (int)fixedRate;

                if (settings.m_Verbose)
                    Debug.Log("Frame recorder set fixed frame rate to " + fixedRate);
            }

            m_Sources = new List<RecorderInput>();
            foreach (var sourceSettings in settings.m_SourceSettings)
            {
                var source = Activator.CreateInstance(sourceSettings.inputType) as RecorderInput;
                source.settings = sourceSettings;
                m_Sources.Add(source);
            }
            return recording = true;
        }

        public virtual void EndRecording(RecordingSession ctx)
        {
            if (!recording)
                return;
            recording = false;

            if (Time.captureFramerate != m_OriginalCaptureFrameRate)
            {
                Time.captureFramerate = (int)m_OriginalCaptureFrameRate;
                if (settings.m_Verbose)
                    Debug.Log("Frame recorder resetting fixed frame rate to original value of " + m_OriginalCaptureFrameRate);
            }

            foreach (var source in m_Sources)
            {
                if (source is IDisposable)
                    (source as IDisposable).Dispose();
            }

            Debug.Log(string.Format("{0} recording stopped, total frame count: {1}", GetType().Name, recordedFramesCount));
        }
        public abstract void RecordFrame(RecordingSession ctx);
        public virtual void PrepareNewFrame(RecordingSession ctx)
        {
        }

        public virtual bool SkipFrame(RecordingSession ctx)
        {
            return !recording || (ctx.m_FrameIndex % settings.m_CaptureEveryNthFrame) != 0;
        }

        public bool recording { get; protected set; }

        public void SignalSourcesOfStage(ERecordingSessionStage stage, RecordingSession session)
        {
            switch (stage)
            {
                case ERecordingSessionStage.BeginRecording:
                    foreach( var source in m_Sources )
                        source.BeginRecording(session);
                    break;
                case ERecordingSessionStage.NewFrameStarting:
                    foreach( var source in m_Sources )
                        source.NewFrameStarting(session);
                    break;
                case ERecordingSessionStage.NewFrameReady:
                    foreach( var source in m_Sources )
                        source.NewFrameReady(session);
                    break;
                case ERecordingSessionStage.FrameDone:
                    foreach( var source in m_Sources )
                        source.FrameDone(session);
                    break;
                case ERecordingSessionStage.EndRecording:
                    foreach( var source in m_Sources )
                        source.EndRecording(session);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage", stage, null);
            }
        }
    }
}
