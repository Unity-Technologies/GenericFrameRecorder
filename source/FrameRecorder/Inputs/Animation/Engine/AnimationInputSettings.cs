using System;
using System.Collections.Generic;

namespace UnityEngine.Recorder.Input
{
    [Serializable]
    [StoreInScene]
    public class AnimationInputSettings : InputSettings<AnimationInput>
    {
        public GameObject gameObject;
        public bool enabled = false;
        public bool recursive = true;
                   
        [HideInInspector]
        public List<string> bindingTypeName = new List<string>();       
        public List<Type> bindingType
        {
            get
            {
                var ret = new List<Type>(bindingTypeName.Count);
                foreach (var t in bindingTypeName)
                {
                    ret.Add( Type.GetType(t));
                }
                return ret;
            }
        }

        public override bool isValid
        {
            get
            {
                return !enabled || gameObject != null && bindingType.Count !=0; 
            }
        }
    }
}
