/*
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.FrameRecorder.Input
{
    
    [Serializable]
    public class AnimationInputComponent:MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public int componentSelectionPopup;

        public Type GetBinding()
        {
            return gameObject.GetComponents<Component>().Select(x => x.GetType()).Distinct().ToList()[componentSelectionPopup];
        }
        private void OnEnable()
        {   
        }


    }
}*/