/*using UnityEditor;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityEngine.FrameRecorder.Input
{
    [ExecuteInEditMode]
    public class AnimationOutputComponent:MonoBehaviour
    {
        [HideInInspector]
        public AnimationRecorderSettings recorderSettings;
        void OnEnable()
        {
            EditorApplication.playmodeStateChanged += PlayStateChange;
        }

        private void OnDisable()
        {
            EditorApplication.playmodeStateChanged -= PlayStateChange;
        }


        void PlayStateChange()
        {
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.Log(EditorApplication.isPlaying);
                EnsureOutputStructure();
                CreateTimelineData();
            }
        }
        
        private void EnsureOutputStructure()
        {
            var director = gameObject.GetComponent<PlayableDirector>();
            if (director == null)
            {
                gameObject.AddComponent<PlayableDirector>();
                director = gameObject.GetComponent<PlayableDirector>();
            }

            var timeline = director.playableAsset;
            if (timeline == null)
            {
                timeline = ScriptableObject.CreateInstance<TimelineAsset>();
                AssetDatabase.CreateAsset(timeline, AnimationRecorder.rootAssetPath+"AnimationRecorder.playable");
                director.playableAsset = timeline;
            }            
        }

        private void CreateTimelineData()
        {
            if (recorderSettings.dirty)
            {
                var director = gameObject.GetComponent<PlayableDirector>();
                var timeline = director.playableAsset as TimelineAsset;
                TrackAsset groupTrack = timeline.CreateTrack<GroupTrack>(null, "Take");
                for (int i = 0;i<recorderSettings.gos.Count;++i)
                {
                    GameObject go = GameObject.Find(recorderSettings.gos[i]);
                    var animTrack = timeline.CreateTrack<AnimationTrack>(groupTrack, go.name);
                    var animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(recorderSettings.animClips[i]);
                    animTrack.CreateClip(animClip).displayName = go.name;
                    director.SetGenericBinding(animTrack,go);
                }

                recorderSettings.dirty = false;
            }
        }
    }
}*/