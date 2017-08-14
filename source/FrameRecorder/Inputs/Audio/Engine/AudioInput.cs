using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

#endif
using UnityEngine.Collections;
using UnityEngine.FrameRecorder.Input;

namespace UnityEngine.FrameRecorder
{
    public class AudioInput : RecorderInput
    {
        private class BufferManager : IDisposable
        {
            NativeArray<float>[] buffers;
            uint m_Length;
	    ushort m_ChannelCount;
    
            public BufferManager(ushort numBuffers, uint length, ushort channelCount)
            {
                buffers = new NativeArray<float>[numBuffers];
		m_ChannelCount = channelCount;
                m_Length = length;
            }
    
            public NativeArray<float> GetBuffer(int index)
            {
                if (buffers[index].Length == 0)
		    buffers[index] = new NativeArray<float>(
			(int)m_Length * (int)m_ChannelCount, Allocator.Temp);
		return buffers[index];
            }
    
            public void Dispose()
            {
                foreach (var a in buffers)
		    a.Dispose();
            }
        }

        public ushort channelCount { get { return m_ChannelCount; } }
	private ushort m_ChannelCount;
	public int sampleRate { get { return AudioSettings.outputSampleRate; } }
	public NativeArray<float> mainBuffer { get { return m_BufferManager.GetBuffer(0); } }
	public NativeArray<float> GetMixerGroupBuffer(int n)
        { return m_BufferManager.GetBuffer(n + 1); }
	
        private BufferManager m_BufferManager;

        public AudioInputSettings audioSettings
        { get { return (AudioInputSettings)settings; } }

        public override void BeginRecording(RecordingSession session)
        {
	    m_ChannelCount = new Func<ushort>(() => {
		    switch (AudioSettings.speakerMode)
		    {
		    case AudioSpeakerMode.Mono:        return 1;
		    case AudioSpeakerMode.Stereo:      return 2;
		    case AudioSpeakerMode.Quad:        return 4;
		    case AudioSpeakerMode.Surround:    return 5;
		    case AudioSpeakerMode.Mode5point1: return 6;
		    case AudioSpeakerMode.Mode7point1: return 7;
		    case AudioSpeakerMode.Prologic:    return 2;		    
		    default: return 1;
		    }
            })();

            if (session.settings.m_Verbose)
                Debug.Log(string.Format(
			      "AudioInput.BeginRecording for capture frame rate {0}", Time.captureFramerate));

	    if (audioSettings.m_PreserveAudio)
		AudioRecorder.StartOutputRecording();
	}

        public override void NewFrameReady(RecordingSession session)
	{
	    if (!audioSettings.m_PreserveAudio)
		return;

	    var sampleFrameCount = (uint)AudioRecorder.GetRecordSamplesAvailable();
            if (session.settings.m_Verbose)
                Debug.Log(string.Format("AudioInput.NewFrameReady {0} audio sample frames @ {1} ch",
                                        sampleFrameCount, m_ChannelCount));

	    ushort bufferCount = 
#if RECORD_AUDIO_MIXERS
		(ushort)(audioSettings.m_AudioMixerGroups.Length + 1)
#else
		1
#endif
            ;

	    m_BufferManager = new BufferManager(bufferCount, sampleFrameCount, m_ChannelCount);
	    var mainBuffer = m_BufferManager.GetBuffer(0);

#if RECORD_AUDIO_MIXERS
	    for (int n = 1; n < bufferCount; n++)
	    {
		var group = audioSettings.m_AudioMixerGroups[n - 1];
		if (group.m_MixerGroup == null)
		    continue;

		var buffer = m_BufferManager.GetBuffer(n);
		AudioRecorder.AddMixerGroupRecorder(group.m_MixerGroup, buffer, group.m_Isolate);
	    }
#endif

	    AudioRecorder.RecordOutput(mainBuffer);
	}

        public override void FrameDone(RecordingSession session)
        {
	    if (!audioSettings.m_PreserveAudio)
		return;

	    m_BufferManager.Dispose();
	    m_BufferManager = null;
	}

        public override void EndRecording(RecordingSession session)
        {
	    if (audioSettings.m_PreserveAudio)
		AudioRecorder.StopOutputRecording();
        }
    }
}

