using UnityEngine.Audio;

namespace UnityEngine.Recorder.Input
{
    public class AudioInputSettings : InputSettings<AudioInput>
    {
        public bool                         m_PreserveAudio = true;

#if RECORD_AUDIO_MIXERS
        [System.Serializable]
        public struct MixerGroupRecorderListItem
        {
            [SerializeField]
            public AudioMixerGroup m_MixerGroup;
            
            [SerializeField]
            public bool m_Isolate;
        }
        public MixerGroupRecorderListItem[] m_AudioMixerGroups;
#endif
        public override bool isValid { get { return true; } }
    }
}
