using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEditor.FrameRecorder
{
    public abstract class InputEditor : Editor
    {
        public delegate EPropertyState IsFieldAvailableDelegate(SerializedProperty property);

        public IsFieldAvailableDelegate IsFieldAvailableForHost { get; set; }

        protected virtual void AddProperty(SerializedProperty prop, Action action )
        {
            var state = IsFieldAvailableForHost(prop);
            if (state == EPropertyState.Enabled)
                state = IsFieldAvailable(prop);
            if (state != EPropertyState.Hidden)
            {
                using (new EditorGUI.DisabledScope(state == EPropertyState.Disabled))
                    action();
            }
        }

        protected virtual EPropertyState IsFieldAvailable( SerializedProperty property)
        {
            return EPropertyState.Enabled;
        }
    }
}
