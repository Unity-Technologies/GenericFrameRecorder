using System;

namespace UnityEngine.FrameRecorder.Input
{
    [Serializable]
    [StoreInScene]
    public class AnimationInputSettings : InputSettings<AnimationInput>
    {

        [SerializeField]
        public GameObject gameObject;
        [SerializeField]
        public bool enabled;
                   
        [SerializeField]
        [HideInInspector]
        public string bindingTypeName;
        

        public Type bindingType
        {
            get { return string.IsNullOrEmpty( bindingTypeName) ? null : Type.GetType(bindingTypeName); }
        }

        public override bool isValid
        {
            get
            {
                return !enabled || gameObject != null && bindingType !=null; 
            }
        }
    }
}
