using System;
using UnityEngine.Recorder.FrameRecorder.Utilities;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.Recorder.FrameRecorder.Timeline
{
    public class FrameRecorderClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        public FrameRecorderSettings m_Settings;

        public Type recorderType
        {
            get { return m_Settings == null ? null : m_Settings.recorderType; }
        }

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<FrameRecorderPlayable>.Create( graph );
            var behaviour = playable.GetBehaviour();
            if (recorderType != null && UnityHelpers.IsPlaying())
            {
                behaviour.session = new RecordingSession()
                {
                    m_Recorder = RecordersInventory.GenerateNewRecorder(recorderType, m_Settings),
                    m_RecorderGO = FrameRecorderGOControler.HookupRecorder(m_Settings),
                    m_RecordingStartTS = Time.time,
                    m_FrameIndex = 0
                };
            }
            return playable;
        }

        public virtual void OnDestroy()
        {
            UnityHelpers.Destroy( m_Settings, true );
        }
    }
}
