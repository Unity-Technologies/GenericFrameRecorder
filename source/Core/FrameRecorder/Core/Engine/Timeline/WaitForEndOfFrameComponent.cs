using System;
using System.Collections;

namespace UnityEngine.FrameRecorder.Timeline
{
    // the purpose of this class is to signal the FrameRecorderPlayable when frame is done.
    [ExecuteInEditMode]
    class WaitForEndOfFrameComponent : MonoBehaviour
    {
        [NonSerialized]
        public RecorderPlayableBehaviour m_playable;

        public IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            if(m_playable != null)
                m_playable.FrameEnded();
        }

        public void LateUpdate()
        {
            StartCoroutine(WaitForEndOfFrame());
        }
    }
}
