using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.FrameRecorder.Timeline
{
    /// <summary>
    /// What is it: Implements a Timeline Clip asset that can be inserted onto a timeline track to trigger a recording of something.
    /// Motivation: Allow Timeline to trigger recordings
    /// 
    /// Note: Instances of this call Own their associated Settings asset's lifetime.
    /// </summary>
    public class RecorderClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        public RecorderSettings m_Settings;

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
            var playable = ScriptPlayable<RecorderPlayableBehaviour>.Create( graph );
            var behaviour = playable.GetBehaviour();
            if (recorderType != null && UnityHelpers.IsPlaying())
            {
                behaviour.session = new RecordingSession()
                {
                    m_Recorder = RecordersInventory.GenerateNewRecorder(recorderType, m_Settings),
                    m_RecorderGO = FrameRecorderGOControler.HookupRecorder(!m_Settings.m_Verbose),
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
