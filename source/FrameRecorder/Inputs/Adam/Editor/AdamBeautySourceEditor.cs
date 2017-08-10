using UnityEngine;
using UnityEngine.FrameRecorder;
using UnityEngine.FrameRecorder.Input;

namespace UnityEditor.FrameRecorder.Input
{
    [CustomEditor(typeof(AdamBeautyInputSettings))]
    public class AdamBeautySourceEditor : InputEditor
    {
        static EImageSource m_SupportedSources = EImageSource.GameDisplay| EImageSource.MainCamera; // | EImageSource.RenderTexture*/; // not sure what to do with the RT as source here.
        string[] m_MaskedSourceNames;
        SerializedProperty m_Source;
        SerializedProperty m_RenderSize;
        SerializedProperty m_RenderTexture;
        SerializedProperty m_FinalSize;
        SerializedProperty m_AspectRatio;
        SerializedProperty m_SuperSampling;

        protected void OnEnable()
        {
            if (target == null)
                return;

            var pf = new PropertyFinder<AdamBeautyInputSettings>(serializedObject);
            m_Source = pf.Find(w => w.source);
            m_RenderSize = pf.Find(w => w.m_RenderSize);
            m_RenderTexture = pf.Find(w => w.m_RenderTexture);
            m_AspectRatio = pf.Find(w => w.m_AspectRatio);
            m_SuperSampling = pf.Find(w => w.m_SuperSampling);
            m_FinalSize = pf.Find(w => w.m_FinalSize);
        }


        public override void OnInspectorGUI()
        {
            AddProperty(m_Source, () =>
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    if (m_MaskedSourceNames == null)
                        m_MaskedSourceNames = EnumHelper.MaskOutEnumNames<EImageSource>((int)m_SupportedSources);
                    var index = EnumHelper.GetMaskedIndexFromEnumValue<EImageSource>(m_Source.intValue, (int)m_SupportedSources);
                    index = EditorGUILayout.Popup("Object(s) of interest", index, m_MaskedSourceNames);

                    if (check.changed)
                        m_Source.intValue = EnumHelper.GetEnumValueFromMaskedIndex<EImageSource>(index, (int)m_SupportedSources);
                }
            });
            
            var inputType = (EImageSource)m_Source.intValue;

            if (inputType != EImageSource.RenderTexture)
            {
                AddProperty(m_AspectRatio, () => EditorGUILayout.PropertyField(m_AspectRatio, new GUIContent("Aspect Ratio")));
                AddProperty(m_SuperSampling, () => EditorGUILayout.PropertyField(m_SuperSampling, new GUIContent("Super sampling")));
            }
            else
            {
                AddProperty(m_RenderTexture, () => EditorGUILayout.PropertyField(m_RenderTexture, new GUIContent("Render Texture")));
                using (new EditorGUI.DisabledScope(true))
                {
                    var res = "N/A";
                    if (m_RenderTexture.objectReferenceValue != null)
                    {
                        var renderTexture = (RenderTexture)m_RenderTexture.objectReferenceValue;
                        res = string.Format("{0} , {1}", renderTexture.width, renderTexture.height);
                    }
                    EditorGUILayout.TextField("Rendering resolution", res);
                }
            }

            var renderSize = m_RenderSize;
            AddProperty(m_RenderSize, () =>
            {
                
                if (inputType != EImageSource.RenderTexture)
                {
                    EditorGUILayout.PropertyField(m_RenderSize, new GUIContent("Rendering resolution"));
                    if (m_FinalSize.intValue > renderSize.intValue)
                        m_FinalSize.intValue = renderSize.intValue;
                }
            });

            AddProperty(m_FinalSize, () => EditorGUILayout.PropertyField(m_FinalSize, new GUIContent("Final resolution")));
            if (m_FinalSize.intValue > renderSize.intValue)
                renderSize.intValue = m_FinalSize.intValue;

            serializedObject.ApplyModifiedProperties();

            if (!(target as AdamBeautyInputSettings).isValid)
            {
                EditorGUILayout.HelpBox("Incomplete/Invalid settings", MessageType.Warning);
            }

        }
    }

}