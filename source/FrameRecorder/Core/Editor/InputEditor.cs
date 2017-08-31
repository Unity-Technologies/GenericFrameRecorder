using System;

namespace UnityEditor.FrameRecorder
{
    public abstract class InputEditor : Editor
    {
        public delegate EFieldDisplayState IsFieldAvailableDelegate(SerializedProperty property);

        public IsFieldAvailableDelegate isFieldAvailableForHost { get; set; }

        protected virtual void AddProperty(SerializedProperty prop, Action action )
        {
            var state = isFieldAvailableForHost == null ? EFieldDisplayState.Disabled : isFieldAvailableForHost(prop);

            if (state == EFieldDisplayState.Enabled)
                state = IsFieldAvailable(prop);
            if (state != EFieldDisplayState.Hidden)
            {
                using (new EditorGUI.DisabledScope(state == EFieldDisplayState.Disabled))
                    action();
            }
        }

        protected virtual EFieldDisplayState IsFieldAvailable( SerializedProperty property)
        {
            return EFieldDisplayState.Enabled;
        }
    }
}
