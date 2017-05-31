using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UnityEditor.FrameRecorder.Input
{
    [CustomEditor(typeof(CBRenderTextureInputSettings))]
    public class CBRenderTextureInputSettingsEditor : Editor
    {
        static EImageSource m_SupportedSources = EImageSource.GameDisplay | EImageSource.MainCamera | EImageSource.RenderTexture;
        string[] m_MaskedSourceNames;
        SerializedProperty m_Source;
        SerializedProperty m_CameraTag;
        SerializedProperty m_RenderSize;
        SerializedProperty m_SourceRTxtr;

        protected void OnEnable()
        {
            if (target == null)
                return;

            var pf = new PropertyFinder<CBRenderTextureInputSettings>(serializedObject);
            m_Source = pf.Find(w => w.source);
            m_CameraTag = pf.Find(w => w.m_CameraTag);
            m_RenderSize = pf.Find(w => w.m_RenderSize);
            m_SourceRTxtr = pf.Find(w => w.m_SourceRTxtr);
        }

        public override void OnInspectorGUI()
        {
            ++EditorGUI.indentLevel;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (m_MaskedSourceNames == null)
                    m_MaskedSourceNames = EnumHelper.MaskOutEnumNames<EImageSource>((int)m_SupportedSources);
                var index = EnumHelper.GetMaskedIndexFromEnumValue<EImageSource>(m_Source.intValue, (int)m_SupportedSources);
                index = EditorGUILayout.Popup("Source", index, m_MaskedSourceNames);

                if (check.changed)
                    m_Source.intValue = EnumHelper.GetEnumValueFromMaskedIndex<EImageSource>(index, (int)m_SupportedSources);
            }

            var inputType = (EImageSource)m_Source.intValue;
            if ((EImageSource)m_Source.intValue == EImageSource.TaggedCamera)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(m_CameraTag, new GUIContent("Tag"));
                --EditorGUI.indentLevel;
            }

            if (inputType != EImageSource.RenderTexture)
            {
                EditorGUILayout.PropertyField(m_RenderSize, new GUIContent("Resolution"));
            }
            else
            {
                EditorGUILayout.PropertyField(m_SourceRTxtr, new GUIContent("RenderTexture"));
                using (new EditorGUI.DisabledScope(true))
                {
                    var res = "N/A";
                    if (m_SourceRTxtr.objectReferenceValue != null)
                    {
                        var renderTexture = (RenderTexture)m_SourceRTxtr.objectReferenceValue;
                        res = string.Format("{0} , {1}", renderTexture.width, renderTexture.height);
                    }
                    EditorGUILayout.TextField("Resolution", res);
                }
            }

            --EditorGUI.indentLevel;
            serializedObject.ApplyModifiedProperties();
        }
    }
}