using System;

namespace UnityEngine.Recorder
{
    /// <summary>
    /// What is it: Place holder for an input setting, in a recorder settings asset, for an input setting that is stored in the scene.
    /// Motivation: Input settings can be flagged to persist in the scene and not in the asset. This is to facilitate settings that target specific scene objects.
    ///             When settings are saved in the scene, need a place holder in the recorder asset that will indicate that it the real settings should be read from the scene.
    /// </summary>
    public class InputBinder : RecorderInputSetting
    {
        public string m_TypeName;

        public override Type inputType
        {
            get { return Type.GetType(m_TypeName); }
        }

        public override bool isValid
        {
            get { return false; }
        }
    }
}
